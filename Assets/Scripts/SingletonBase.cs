using System;
using UnityEngine;

// /* SingletonBase<T> 클래스는 싱글톤 패턴을 적용한 클래스의 기반 클래스이다.
//  * T는 제네릭 타입으로, 이 클래스를 상속받는 클래스의 타입을 지정한다.
//  * 이 클래스를 상속받는 클래스는 GetSingleton() 메소드를 통해 싱글톤 인스턴스를 가져올 수 있다.
//  * 
//  * 싱글톤 패턴은 어떤 클래스가 단 하나의 인스턴스만을 가지도록 보장하는 디자인 패턴이다.
//  * 이 패턴을 사용하면 여러 곳에서 동일한 인스턴스를 참조할 수 있어 메모리를 절약할 수 있다.
//  * 
//  * 이 클래스는 MonoBehaviour를 상속받아 사용하며, Awake() 메소드를 통해 싱글톤 인스턴스를 생성한다.
//  * 이미 생성된 인스턴스가 있을 경우, 새로 생성하는 대신 해당 인스턴스를 반환한다.
//  * 
//  * 싱글톤 패턴은 다음과 같은 경우에 사용된다.
//  * - 게임 내에서 단 하나의 인스턴스만 필요한 클래스
//  * - 여러 곳에서 동일한 인스턴스를 참조해야 하는 클래스
//  * - 메모리를 절약하고 싶은 클래스
//  * 
//  * 사용 예시:
//  * PlayerInfo.GetSingleton().SetPlayerType(PlayerType.Boy);
//  */
// public class SingletonBase<T> : MonoBehaviour where T : class, new()
// {
//     protected static T _instance = null;
//     public static T GetSingleton()
//     {
//         if (_instance == null)
//         {
//             Debug.LogError("Singleton Class is null: " + typeof(T));
//             return null;
//         }

//         return _instance;
//     }

//     void Awake()
//     {
//         if (_instance == null)
//         {
//             _instance = this as T;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }
// }

public abstract class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour
{
    private static readonly Lazy<T> _lazy = new Lazy<T>(GetSingleton);

    public static T Instance => _lazy.Value;

    private static T GetSingleton()
    {
        T monobehaviourInstance = FindObjectOfType<T>();
        if (monobehaviourInstance == null)
        {
            GameObject gameObj = new GameObject(typeof(T).Name);
            monobehaviourInstance = gameObj.AddComponent<T>();
            DontDestroyOnLoad(gameObj);
        }
        return monobehaviourInstance;
    }
    
    // 필요시 Unity Message를 통한 부가적인 처리
    protected virtual void Awake() { }
    protected virtual void OnDestroy() { }
    protected virtual void OnApplicationQuit() { }
}

// // 사용시
// internal sealed class DerivedSingletonMono : SingletonMonoBase<DerivedSingletonMono>
// {
//     //protected override void Awake()
//     //{
//     //    base.Awake();
//     //    // 필요시 부가적인 처리 ...
//     //}

//     //protected override void OnApplicationQuit()
//     //{
//     //    base.OnApplicationQuit();
//     //    // ...
//     //}

//     //protected override void OnDestroy()
//     //{
//     //    base.OnDestroy();
//     //    // ...
//     //}
// }