using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thiefClick: MonoBehaviour
{
  public delegate void ThiefClickedAction();
    public static event ThiefClickedAction OnThiefClicked;

    private void OnMouseDown()
    {
        OnThiefClicked?.Invoke(); // 클릭 시 이벤트 호출
        Destroy(gameObject); // 도둑을 클릭하면 사라지게 함
    }
}
