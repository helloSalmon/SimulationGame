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
using DG.Tweening; 

public interface ICraneController
{
    public Transform Container { get; }
} 

public class NewCraneController : MonoBehaviour, ICraneController
{
    float rotationSpeed = 60.0f;
    float extrusionSpeed = 10.0f;
    float armMinScale = 20.0f;
    float armMaxScale = 50.0f;
    const float hookOffset = 6.0f;
    public Vector3 hookDir = Vector3.forward;

    [SerializeField]
    Transform arm, head, hook, joint;
    [SerializeField]
    LineRendererAtoB rope_;
    Transform container;
    public Transform Container { get { return container; } }

    [SerializeField]
    VoxelBuildingSystem buildingSystem;

    public bool isSelected = false;
    bool hasHit = false;
    Vector3 _destPos;
    GameObject plate;

    // for logic
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
        _destPos = arm.position - transform.position;
        _destPos.y = 0;
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
        float armLength = Mathf.Abs(arm.localPosition.z);
        float positive = Mathf.Sign(arm.localPosition.z);
        if (Mathf.Abs(dir.magnitude - armLength) > 0.1f)
        {
            if (dir.magnitude > armLength)
                extension = positive;
            else if (dir.magnitude < armLength)
                extension = -positive;
        }
        var angle = Quaternion.Angle(head.rotation, Quaternion.LookRotation(dir));
        if (Vector3.Cross(head.forward, dir).y > 0)
        {
            angle = -angle;
        }
        if (180 - Mathf.Abs(angle) > 0.1f)
        {
            head.localRotation *= Quaternion.AngleAxis(Mathf.Sign(angle) * rotationSpeed * Time.deltaTime, Vector3.up);
        }

        // hook와 rope의 position을 arm의 extrusion 정도에 따라 조정해야 한다
        Vector3 temp = arm.localPosition;
        temp.z += extension * 3 * extrusionSpeed * Time.deltaTime;
        if (Mathf.Abs(temp.z) < armMaxScale && Mathf.Abs(temp.z) > armMinScale)
            arm.localPosition = temp;
    }


    void UpdateDropping()
    {
        if (distance > 0.01f)
        {
            rope_.AddLength(extrusionSpeed * Time.deltaTime);
            hook.localPosition = new Vector3(hook.localPosition.x, -rope_.Length - 1, hook.localPosition.z);
            distance -= extrusionSpeed * Time.deltaTime;
        }
        else
        {
            _state = CraneState.Moving;
        }
    }

    void UpdateRaising()
    {
        if (rope_.CanShorten())
        {
            rope_.AddLength(-extrusionSpeed * Time.deltaTime);
            hook.localPosition = new Vector3(hook.localPosition.x, -rope_.Length - 1, hook.localPosition.z);
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

    public void UpdateSpinning(int rotating = 0)
    {
        joint.rotation = Quaternion.Lerp(joint.rotation, Quaternion.LookRotation(hookDir), rotationSpeed * Time.deltaTime);

        if (!isSelected)
            return;

        if (Input.GetKeyDown(KeyCode.C) || rotating == 1)
        {
            joint.DORotateQuaternion(Quaternion.LookRotation(Vector3.forward), 2f);
            hookDir = Vector3.forward;
            // joint.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.V) || rotating == 2)
        {
            joint.DORotateQuaternion(Quaternion.LookRotation(Vector3.right), 2f);
            hookDir = Vector3.right;
            // joint.Rotate(Vector3.down, rotationSpeed * Time.deltaTime);
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

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
            OnKeyPressed();
            OnMouseClicked();
        }
        switch (_state)
        {
            case CraneState.Moving:
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
        if (container != null) return false;
        Vector3 origin = hook.position;
        origin.y = 0;
        VoxelBehaviour voxelBehaviour = null;
        Vector3 offset = Vector3.up * 50f;
        plate = null;
        int size = Physics.RaycastNonAlloc(hook.position + offset, -hook.up, Buffer.raycastHit, 100.0f, LayerMask.GetMask("Floor", "Block", "VoxelSpace"));
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
            else if (target.tag == "Container")
            {
                float temp = hit.distance - hookOffset - offset.y;
                if (min_distance > temp)
                {
                    min_distance = temp;
                    container = hit.collider.transform;
                    poObject = target.GetComponent<PlaceableObject>();
                }
            }
            else if (target.gameObject.layer == LayerMask.NameToLayer("VoxelSpace"))
            {
                voxelBehaviour = target.gameObject.GetComponent<VoxelBehaviour>();
            }

        }


        voxelActive = false;
        if (voxelBehaviour != null && poObject != null)
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
        // VoxelBehaviour voxelBehaviour = SearchVoxel(origin);
        VoxelBehaviour voxelBehaviour = null;
        Vector3 offset = Vector3.up * 50f;
        int size = Physics.RaycastNonAlloc(container.position + offset, -container.up, Buffer.raycastHit, 100.0f, LayerMask.GetMask("Floor", "Block", "VoxelSpace"));
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
                //float temp = hit.distance;
                min_distance = Mathf.Min(min_distance, hit.distance - ((Container)poObject).Size.y / 2 - offset.y);
            }
            else if (target.gameObject.layer == LayerMask.NameToLayer("Block"))
            {
                //min_distance = Mathf.Min(min_distance, hit.distance);
                float temp = hit.distance - ((Container)poObject).Size.y / 2 - offset.y;
                if (temp > 0)
                    min_distance = Mathf.Min(min_distance, temp);
            }
            else if (target.gameObject.layer == LayerMask.NameToLayer("VoxelSpace"))
            {
                voxelBehaviour = target.gameObject.GetComponent<VoxelBehaviour>();
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

    bool CheckHookFit()
    {
        float angle = Quaternion.Angle(container.rotation, joint.rotation);

        Vector3 hookPos = hook.position;
        hookPos.y = 0;

        Vector3 containerPos = container.position;
        containerPos.y = 0;

        if (angle > 1.0f)
            return false;

        if ((containerPos - hookPos).magnitude > Vector3.one.magnitude)
            return false;

        return true;
    }

    public void OnKeyPressed(int updown = 0)
    {
        Debug.DrawRay(hook.position, -hook.up * 30, Color.red);
        if (container)
            Debug.DrawRay(container.position, -container.up * 30, Color.blue);
        if (_state == CraneState.Moving && isSelected)
        {
            if (Input.GetKey(KeyCode.Z) || updown == 1)
            {
                if (CanAttach())
                {
                    if (!CheckHookFit())
                    {
                        container = null;
                        return;
                    }
                    StartCoroutine(GrabContainer());
                }
            }
            else if (Input.GetKey(KeyCode.X) || updown == 2)
            {
                if (CanDetach())
                {
                    StartCoroutine(ReleaseContainer());
                }
            }
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
                if (hit.collider.gameObject.tag == "Crane")
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
                    if (container == null && hit.collider.gameObject.tag == "Container")
                        _destPos = hit.collider.transform.position;
                    else 
                        _destPos = hit.point;
                    _destPos -= head.position;
                }
            }
        }
    }

    public void OnSelected()
    {
        _state = CraneState.Moving;
        _check.selectedCrane = this;
        _light.Hightlight(true);
    }

    public void OnDeselected()
    {
        _light.Hightlight(false);
    }

    IEnumerator ResetHitFlagAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        hasHit = false;
    }
}
