using R3;
using UnityEngine;

public class TankPresenter : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] private TankStatus tankStatus;

    [Header("View")]
    [SerializeField] private GetModuleSelectUI getModuleSelectUI;


    void Start()
    {
        //!:現在未完 実装予定。
        //do:UI部分の実装が完了次第、tankstatusをUIのシステムへ通知するようにする。

        //:tankStatus.HP.Subscribe(hp => view.UpdateHP(hp, tankStatus.maxHP)).AddTo(this);
    }
}
