using System.Collections;
using UnityEngine;



/// <summary>
/// MonoBehaviour를 상속받지 않아 코루틴을 직접 사용할 수 없는 객체 대신 코루틴을 실행해주는 서비스입니다.
/// </summary>
public interface ICoroutineRunner
{
    Coroutine RequestStartCoroutine(IEnumerator enumerator);
}



/// <summary>
/// MonoBehaviour를 상속받지 않아 코루틴을 직접 사용할 수 없는 객체 대신 코루틴을 실행해주는 컴포넌트입니다.
/// </summary>
public class CoroutineRunner : MonoBehaviour, ICoroutineRunner
{

    public Coroutine RequestStartCoroutine(IEnumerator enumerator)
    {
        return RequestStartCoroutine(enumerator);
    }
}
