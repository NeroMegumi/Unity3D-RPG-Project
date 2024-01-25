using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class MouseManager : Singleton<MouseManager>
{
    public RaycastHit hitinfo;
    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> AttackEnemy;
    public Texture2D Point, Doorway, Attack, Target, Arrow;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        SetCursorTexture();
        MouseControl();
    }
    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray,out hitinfo))
        {
            //更改鼠标图标
            switch (hitinfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(Target,new Vector2(16,16),CursorMode.ForceSoftware);
                    break;
                case "Enemy":
                    Cursor.SetCursor(Attack, new Vector2(16, 16), CursorMode.ForceSoftware);
                    break;
                case "Portal":
                    Cursor.SetCursor(Doorway, new Vector2(16, 16), CursorMode.ForceSoftware);
                    break;

            }
        }
    }
    /// <summary>
    /// 获得鼠标位置
    /// </summary>
    void MouseControl()
    {
        if(Input.GetMouseButtonDown(0) && hitinfo.collider != null)//左键单击且没有点到虚空
        {
            if(hitinfo.collider.gameObject.CompareTag("Ground"))//射线碰撞物体的游戏对象的Tag是Ground（记得给Ground加上这个tag）
                OnMouseClicked?.Invoke(hitinfo.point);//OnMouseCliked是否为空，不空就传入一个Vector3（传给AI Navigation的destination让其自动寻路）
            if (hitinfo.collider.gameObject.CompareTag("Enemy"))//射线碰撞物体的游戏对象的Tag是Enemy（记得给Enemy加上这个tag）
                AttackEnemy?.Invoke(hitinfo.collider.gameObject);
            if (hitinfo.collider.gameObject.CompareTag("Attackable"))//射线碰撞物体的游戏对象的Tag是Attackable（记得给Rock加上这个tag）
                AttackEnemy?.Invoke(hitinfo.collider.gameObject);
            if (hitinfo.collider.gameObject.CompareTag("Portal"))
                OnMouseClicked?.Invoke(hitinfo.point);
        }
    }
}


