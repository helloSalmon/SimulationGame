using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class CraneController : MonoBehaviour
{
    // Start is called before the first frame update
    float rotationSpeed = 60.0f;
    float extrusionSpeed = 10.0f;
    float ropeMinScale = 4.0f;
    float ropeMaxScale = 30.0f;
    float armMinScale = 10.0f;
    float armMaxScale = 30.0f;

    [SerializeField]
    Transform arm, head, hook, rope, container;

    [SerializeField]
    VoxelBuildingSystem buildingSystem;

    bool isSelected = false;
    bool hasHit = false;
    bool hasContainer = false;
    Vector3 _destPos;
    GameObject location;

    // for logic
    float containerHookDistance;
    float containerFloorDistance;
    float distance;

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
        head.localRotation = Quaternion.Slerp(head.localRotation, Quaternion.LookRotation(dir), 0.01f);

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

    IEnumerator GrabContainer(GameObject go)
    {
        _state = CraneState.Dropping;
        while (_state == CraneState.Dropping)
        {
            yield return null;
        }
        AttachContainer(go);
        _state = CraneState.Raising;
        while (_state == CraneState.Raising)
        {

            yield return null;
        }
        hasContainer = true;
        _state = CraneState.Moving;
    }

    IEnumerator ReleaseContainer(GameObject go)
    {
        _state = CraneState.Dropping;
        while (_state == CraneState.Dropping)
        {
            yield return null;
        }
        DetachContainer(go);
        _state = CraneState.Raising;
        while (_state == CraneState.Raising)
        {
            yield return null;
        }
        hasContainer = false;
        _state = CraneState.Moving;
    }

    void UpdateSpinning()
    {
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

    void AttachContainer(GameObject go)
    {
        if (go != null)
            go.GetComponent<ContainerLocation>().myContainer = null;

        int size = Physics.OverlapSphereNonAlloc(container.transform.position, 1, Buffer.colliderBuffer, 1 << LayerMask.GetMask("VoxelSpace"));
        PlaceableObject poContainer = container.GetComponent<PlaceableObject>();

        for (int i = 0; i < size; i++)
        {
            Collider target = Buffer.colliderBuffer[i];

            if (!target.TryGetComponent(out VoxelBehaviour voxelBehaviour))
                continue;

            Voxel<VoxelObject> voxel = voxelBehaviour.voxel;

            buildingSystem.RemoveBuilding(poContainer, voxel);

            break;
        }

        container.SetParent(hook);
        Debug.Log("Connected");
    }

    void DetachContainer(GameObject go)
    {
        if (go != null)
            go.GetComponent<ContainerLocation>().myContainer = container.gameObject;

        int size = Physics.OverlapSphereNonAlloc(container.transform.position, 1, Buffer.colliderBuffer, 1 << LayerMask.GetMask("VoxelSpace"));
        PlaceableObject poContainer = container.GetComponent<PlaceableObject>();

        for (int i = 0; i < size; i++)
        {
            Collider target = Buffer.colliderBuffer[i];

            if (!target.TryGetComponent(out VoxelBehaviour voxelBehaviour))
                continue;

            Voxel<VoxelObject> voxel = voxelBehaviour.voxel;

            Debug.Log(PlaceableObject.GetDirectonFromRotation(container.transform.rotation).ToString());

            buildingSystem.PlaceBuilding(voxel,
                                         poContainer,
                                         voxelBehaviour.WorldToCell(container.transform.position),
                                         PlaceableObject.GetDirectonFromRotation(container.transform.rotation));

            container.transform.position = voxelBehaviour.GetPlaceableObjectCenterWorld(poContainer);

            break;
        }

        container.SetParent(null);
        container = null;
        Debug.Log("Disconnected");
    }

    // Update is called once per frame
    void Update()
    {

        OnKeyPressed();
        OnMouseMove();
        OnMouseClicked();
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

    void OnKeyPressed()
    {
        if (_state == CraneState.Moving && Input.anyKeyDown && isSelected)
        {
            RaycastHit hit;

            if (Input.GetKey(KeyCode.Z))
            {
                if (Physics.Raycast(hook.position, -hook.up, out hit, 30.0f))
                {
                    if (hit.collider != null && hit.collider.gameObject.name == "Container(Clone)")
                    {
                        Debug.Log("Find Object: " + hit.collider.gameObject.name);
                        containerHookDistance = hit.distance;
                        distance = containerHookDistance - hook.localScale.y / 2;
                        container = hit.collider.transform;
                        // Debug.Log("Hit Distance: " + distance);

                        location = null;
                        if (Physics.Raycast(hook.position, -hook.up, out hit, 30.0f, LayerMask.GetMask("Floor")))
                            location = hit.collider.gameObject;

                        StartCoroutine(GrabContainer(location));
                    }
                }
            }
            else if (Input.GetKey(KeyCode.X) && hasContainer)
            {
                if (Physics.Raycast(container.position, -container.up, out hit, 30.0f))
                {
                    if (hit.collider != null)
                    {
                        location = null;
                        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
                            location = hit.collider.gameObject;
                        Debug.Log("Find Object: " + hit.collider.gameObject.name);
                        containerFloorDistance = hit.distance;
                        distance = containerFloorDistance - container.localScale.y / 2;
                        // Debug.Log("Hit Distance: " + distance);
                        StartCoroutine(ReleaseContainer(location));
                    }
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
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.collider.gameObject.name == "Crane")
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        isSelected ^= true;
                        hasHit = true;
                        if (isSelected)
                            _state = CraneState.Moving;
                        else
                            _state = CraneState.Stopped;
                        StartCoroutine(ResetHitFlagAfterSeconds(0.1f));
                    }
                    else
                    {
                        isSelected = false;
                    }
                    Debug.Log($"Selected : {isSelected}");
                }
                else if (isSelected)
                {
                    _destPos = hit.point;
                    _destPos -= transform.position;
                    // Debug.Log($"Hit : {_destPos}");
                }
            }
        }
    }

    IEnumerator ResetHitFlagAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        hasHit = false;
    }
}
