using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class CraneController : MonoBehaviour, ICraneController
{
    // Start is called before the first frame update
    float rotationSpeed = 60.0f;
    float extrusionSpeed = 10.0f;
    float ropeMinScale = 4.0f;
    float ropeMaxScale = 30.0f;
    float armMinScale = 10.0f;
    float armMaxScale = 30.0f;

    [SerializeField]
    Transform arm, head, hook, rope;
    Transform container;
    public Transform Container { get { return container; } }

    [SerializeField]
    VoxelBuildingSystem buildingSystem;

    bool isSelected = false;
    bool hasHit = false;
    Vector3 _destPos;
    GameObject plate;

    // for logic
    float containerHookDistance;
    float containerFloorDistance;
    float distance;

    // temporary
    CheckContainer _check;
    bool voxelActive = false;
    Highlighter _light;

    public enum CraneState
    {
        Dropping,
        Raising,
        Moving,
        Stopped,
    }
    CraneState _state;

    void Start()
    {
        _destPos = hook.localPosition;
        _destPos = new Vector3(_destPos.x, 0, _destPos.z);
        _state = CraneState.Stopped;
        _check = GameObject.Find("Check Container Button").GetComponent<CheckContainer>();
        _light = gameObject.GetComponent<Highlighter>();
    }

    void Extrude(Transform trf, float extrusion, float extrusionSpeed, Vector3 dir, float minScale, float maxScale)
    {
        Vector3 tmp = Vector3.Scale(dir, dir);
        Vector3 initialScale = Vector3.Scale(trf.localScale, tmp);
        float scaleIncrement = extrusion * extrusionSpeed * Time.deltaTime;
        scaleIncrement += initialScale.magnitude;
        // scaleIncrement는 변화한 후 scale의 절댓값

        scaleIncrement = Mathf.Clamp(scaleIncrement, minScale, maxScale);
        Vector3 newScale = tmp * scaleIncrement;

        trf.localScale = Vector3.Scale(Vector3.one - tmp, trf.localScale) + newScale;

        // 만약 음수 방향으로 extrude 하려면 position이 반대로 움직여야 한다
        if (dir.x < 0 || dir.y < 0 || dir.z < 0)
            trf.localPosition -= (newScale - initialScale) / 2.0f;
        else
            trf.localPosition += (newScale - initialScale) / 2.0f;
    }

    void UpdateMoving()
    {
        Vector3 dir = new Vector3(_destPos.x, 0, _destPos.z);
        float extension = 0.0f;
        if (Mathf.Abs(dir.magnitude - hook.localPosition.z) > 0.1f)
        {
            if (dir.magnitude > hook.localPosition.z)
                extension = 1.0f;
            else if (dir.magnitude < hook.localPosition.z)
                extension = -1.0f;
        }
        // head.localRotation = Quaternion.Lerp(head.localRotation, Quaternion.LookRotation(dir), 0.01f);
        var angle = Quaternion.Angle(head.rotation, Quaternion.LookRotation(dir));
        if (Vector3.Cross(head.forward, dir).y > 0)
        {
            angle = -angle;
        }
        if (Mathf.Abs(angle) > 0.01f)
        {
            head.localRotation *= Quaternion.AngleAxis(-Mathf.Sign(angle) * rotationSpeed * Time.deltaTime, Vector3.up);
            // head.rotation = Quaternion.RotateTowards(head.rotation, Quaternion.Euler(0, angle, 0), rotationSpeed * Time.deltaTime);
        }

        // hook와 rope의 position을 arm의 extrusion 정도에 따라 조정해야 한다
        Extrude(arm, extension, extrusionSpeed * 4, Vector3.forward, armMinScale, armMaxScale);
        hook.localPosition = new Vector3(hook.localPosition.x, hook.localPosition.y, arm.localScale.z + 1.5f);
        rope.localPosition = new Vector3(rope.localPosition.x, rope.localPosition.y, arm.localScale.z + 1.5f);
    }


    void UpdateDropping()
    {
        if (distance > 0.01f)
        {
            Extrude(rope, 1.0f, extrusionSpeed, Vector3.down, ropeMinScale, ropeMaxScale);
            hook.localPosition = new Vector3(hook.localPosition.x, -rope.localScale.y - 0.3f, hook.localPosition.z);
            distance -= extrusionSpeed * Time.deltaTime;
        }
        else
        {
            _state = CraneState.Moving;
        }
    }

    void UpdateRaising()
    {
        if (rope.localScale.y - ropeMinScale > 0.1f)
        {
            Extrude(rope, -1.0f, extrusionSpeed, Vector3.down, ropeMinScale, ropeMaxScale);
            hook.localPosition = new Vector3(hook.localPosition.x, -rope.localScale.y - 0.3f, hook.localPosition.z);
        }
        else
        {
            _state = CraneState.Moving;
        }
    }

    IEnumerator GrabContainer()
    {
        _state = CraneState.Dropping;
        while (_state == CraneState.Dropping)
        {
            yield return null;
        }
        AttachContainer();
        _state = CraneState.Raising;
        while (_state == CraneState.Raising)
        {

            yield return null;
        }
        _state = CraneState.Moving;
    }

    IEnumerator ReleaseContainer()
    {
        _state = CraneState.Dropping;
        while (_state == CraneState.Dropping)
        {
            yield return null;
        }
        DetachContainer();
        _state = CraneState.Raising;
        while (_state == CraneState.Raising)
        {
            yield return null;
        }
        _state = CraneState.Moving;
    }

    void UpdateSpinning()
    {
        if (!isSelected)
            return;

        if (Input.GetKey(KeyCode.C))
        {
            // hook.localRotation *= Quaternion.Euler(0.0f, 1.1f, 0.0f);
            hook.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.V))
        {
            // hook.localRotation *= Quaternion.Euler(0.0f, -1.1f, 0.0f);
            hook.Rotate(Vector3.down, rotationSpeed * Time.deltaTime);
        }
    }

    void AttachContainer()
    {
        if (plate != null)
            plate.GetComponent<ContainerLocation>().myContainer = null;

        if (voxelActive)
        {
            PlaceableObject poContainer = container.GetComponent<PlaceableObject>();
            VoxelBehaviour voxelBehaviour = SearchVoxel(container.transform.position);
            Voxel<VoxelObject> voxel = voxelBehaviour.voxel;

            buildingSystem.RemoveBuilding(poContainer, voxel);
        }

        container.SetParent(hook);
        Debug.Log("Connected");
    }

    void DetachContainer()
    {
        if (plate != null)
            plate.GetComponent<ContainerLocation>().myContainer = container.gameObject;

        if (voxelActive)
        {
            PlaceableObject poContainer = container.GetComponent<PlaceableObject>();

            VoxelBehaviour voxelBehaviour = SearchVoxel(container.transform.position);
            Voxel<VoxelObject> voxel = voxelBehaviour.voxel;

            Debug.Log(PlaceableObject.GetDirectonFromRotation(container.transform.rotation).ToString());

            buildingSystem.PlaceBuilding(voxel,
                                         poContainer,
                                         voxelBehaviour.WorldToCell(container.transform.position),
                                         PlaceableObject.GetDirectonFromRotation(container.transform.rotation));

            container.transform.position = voxelBehaviour.GetPlaceableObjectCenterWorld(poContainer);
        }

        container.SetParent(null);
        container = null;
        Debug.Log("Disconnected");
    }

    VoxelBehaviour SearchVoxel(Vector3 position, float radius = 1f)
    {
        
        int size = Physics.OverlapSphereNonAlloc(position, radius, Buffer.colliderBuffer, LayerMask.GetMask("VoxelSpace"));

        for (int i = 0; i < size; i++)
        {
            Collider target = Buffer.colliderBuffer[i];

            if (target.TryGetComponent(out VoxelBehaviour voxelBehaviour))
            {
                return voxelBehaviour;
            }
        }

        return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
            OnKeyPressed();
            OnMouseMove();
            OnMouseClicked();
        }
        switch (_state)
        {
            case CraneState.Moving:
                //if (isSelected)
                //    _state = CraneState.Moving;
                //else
                //    _state = CraneState.Stopped;
                UpdateMoving();
                UpdateSpinning(); break;
            case CraneState.Dropping:
                UpdateDropping(); break;
            case CraneState.Raising:
                UpdateRaising(); break;
            default: break;
        }
    }

    bool CanAttach()
    {
        Vector3 origin = hook.position;
        origin.y = 0;
        VoxelBehaviour voxelBehaviour = SearchVoxel(origin);
        plate = null;
        int size = Physics.RaycastNonAlloc(hook.position, -hook.up, Buffer.raycastHit, 30.0f, LayerMask.GetMask("Floor", "Block", "VoxelSpace"));
        PlaceableObject poObject = null;

        float min_distance = float.MaxValue;

        for (int i = 0; i < size; ++i)
        {
            RaycastHit hit = Buffer.raycastHit[i];
            Collider target = hit.collider;

            if (target.gameObject.layer == LayerMask.NameToLayer("Floor"))
            {
                plate = target.gameObject;
            }
            else if (target.name == "Container")
            {
                float temp = hit.distance - hook.localScale.y / 2;
                if (min_distance > temp)
                {
                    min_distance = temp;
                    container = hit.collider.transform;
                    poObject = target.GetComponent<PlaceableObject>();
                }
            }
        }


        voxelActive = false;
        if (voxelBehaviour != null)
        {
            voxelActive = buildingSystem.CanRemove(poObject, voxelBehaviour.voxel);
            if (!voxelActive)
                return false;
        }

        if (poObject != null)
        {
            distance = min_distance;
            return true;
        }

        return false;
    }

    bool CanDetach()
    {
        if (container == null) return false;

        Vector3 origin = container.position;

        // 나중에 바닥의 높이에 따라 origin을 높여줘야 함
        origin.y = 0;
        VoxelBehaviour voxelBehaviour = SearchVoxel(origin);
        int size = Physics.RaycastNonAlloc(container.position, -container.up, Buffer.raycastHit, 30.0f, LayerMask.GetMask("Floor", "Block", "VoxelSpace"));
        PlaceableObject poObject = container.GetComponent<PlaceableObject>();

        plate = null;
        float min_distance = float.MaxValue;

        for (int i = 0; i < size; ++i)
        {
            Collider target = Buffer.raycastHit[i].collider;
            RaycastHit hit = Buffer.raycastHit[i];

            if (target.gameObject.layer == LayerMask.NameToLayer("Floor"))
            {
                plate = target.gameObject;
                float temp = hit.distance - container.localScale.y / 2;
                if (min_distance > temp)
                {
                    min_distance = temp;
                }
            }
            else if (target.gameObject.layer == LayerMask.NameToLayer("Block"))
            {
                min_distance = Mathf.Min(min_distance, hit.distance - container.localScale.y / 2);
            }
        }

        if (float.MaxValue - min_distance < 0.1f)
        {
            min_distance = container.position.y;
        }

        origin.y = container.position.y - min_distance;
        voxelActive = false;
        if (voxelBehaviour != null)
        {
            voxelActive = buildingSystem.CanBuild(voxelBehaviour.voxel, poObject,
                                                  voxelBehaviour.WorldToCell(origin),
                                                  PlaceableObject.GetDirectonFromRotation(container.transform.rotation));
            if (!voxelActive)
                return false;
        }

        if (min_distance < float.MaxValue)
        {
            distance = min_distance;
            return true;
        }

        return false;
    }

    void OnKeyPressed()
    {
        Debug.DrawRay(hook.position, -hook.up * 30, Color.red);
        if (container)
            Debug.DrawRay(container.position, -container.up * 30, Color.blue);
        if (_state == CraneState.Moving && Input.anyKeyDown && isSelected)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                if (CanAttach())
                {
                    //Debug.Log("Find Object: " + hit.collider.gameObject.name);
                    //containerHookDistance = hit.distance;
                    //distance = containerHookDistance - hook.localScale.y / 2;
                    //container = hit.collider.transform;
                    //// Debug.Log("Hit Distance: " + distance);

                    //plate = null;
                    //if (Physics.Raycast(hook.position, -hook.up, out hit, 30.0f, LayerMask.GetMask("Floor")))
                    //    plate = hit.collider.gameObject;

                    StartCoroutine(GrabContainer());
                }
            }
            else if (Input.GetKey(KeyCode.X))
            {
                if (CanDetach())
                {
                    //plate = null;
                    //if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
                    //    plate = hit.collider.gameObject;
                    //Debug.Log("Find Object: " + hit.collider.gameObject.name);
                    //containerFloorDistance = hit.distance;
                    //distance = containerFloorDistance - container.localScale.y / 2;
                    //// Debug.Log("Hit Distance: " + distance);
                    StartCoroutine(ReleaseContainer());
                }
            }
        }
    }

    void OnMouseMove()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (_state == CraneState.Moving && mouseX != 0 && mouseY != 0)
        {
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;
            //if (Physics.Raycast(ray, out hit, 100.0f))
            //{
            //    _destPos = new Vector3(hit.point.x, 0, hit.point.z);
            //}
        }
    }

    void OnMouseClicked()
    {
        if (Input.GetMouseButtonDown(0) && !hasHit)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 300f, LayerMask.GetMask("Block")))
            {
                if (hit.collider.gameObject.name == "Crane")
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        isSelected ^= true;
                        hasHit = true;
                        if (isSelected)
                            OnSelected();
                        else
                            OnDeselected();
                        StartCoroutine(ResetHitFlagAfterSeconds(0.1f));
                    }
                    else
                    {
                        isSelected = false;
                        OnDeselected();
                    }
                    Debug.Log($"Selected : {isSelected}");
                }
                else if (isSelected)
                {
                    _destPos = hit.point;
                    _destPos -= head.position;
                    // Debug.Log($"Hit : {_destPos}");
                }
            }
        }
    }

    private void OnSelected()
    {
        _state = CraneState.Moving;
        _check.selectedCrane = this;
        _light.Hightlight(true);
    }

    private void OnDeselected()
    {
        // _state = CraneState.Stopped;
        _light.Hightlight(false);
    }

    IEnumerator ResetHitFlagAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        hasHit = false;
    }
}
