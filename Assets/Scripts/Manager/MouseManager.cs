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
            //�������ͼ��
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
    /// ������λ��
    /// </summary>
    void MouseControl()
    {
        if(Input.GetMouseButtonDown(0) && hitinfo.collider != null)//���������û�е㵽���
        {
            if(hitinfo.collider.gameObject.CompareTag("Ground"))//������ײ�������Ϸ�����Tag��Ground���ǵø�Ground�������tag��
                OnMouseClicked?.Invoke(hitinfo.point);//OnMouseCliked�Ƿ�Ϊ�գ����վʹ���һ��Vector3������AI Navigation��destination�����Զ�Ѱ·��
            if (hitinfo.collider.gameObject.CompareTag("Enemy"))//������ײ�������Ϸ�����Tag��Enemy���ǵø�Enemy�������tag��
                AttackEnemy?.Invoke(hitinfo.collider.gameObject);
            if (hitinfo.collider.gameObject.CompareTag("Attackable"))//������ײ�������Ϸ�����Tag��Attackable���ǵø�Rock�������tag��
                AttackEnemy?.Invoke(hitinfo.collider.gameObject);
            if (hitinfo.collider.gameObject.CompareTag("Portal"))
                OnMouseClicked?.Invoke(hitinfo.point);
        }
    }
}


