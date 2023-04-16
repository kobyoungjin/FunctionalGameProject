using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public static float ACCELERATION = 10.0f; // ���ӵ�
    public static float SPEED_MIN = 4.0f; // �ӵ��� �ּڰ�
    public static float SPEED_MAX = 8.0f; // �ӵ��� �ִ�
    public static float JUMP_HEIGHT_MAX = 3.0f; // ���� ����
    public static float JUMP_KEY_RELEASE_REDUCE = 0.5f; // ���� ���� ���ӵ�

    public enum STEP
    { // Player�� ���� ���¸� ��Ÿ���� �ڷ��� (����ü)
        NONE = -1, // �������� ����
        RUN = 0, // �޸�
        JUMP, // ����
        MISS, // ����
        NUM, // ���°� �� ���� �ִ��� ������(=3)
    };

    public STEP step = STEP.NONE; // Player�� ���� ����
    public STEP next_step = STEP.NONE; // Player�� ���� ����
    public float step_timer = 0.0f; // ��� �ð�
    private bool is_landed = false; // �����ߴ°�
    private bool is_colided = false; // ������ �浹�ߴ°�
    private bool is_key_released = false; // ��ư�� �������°�

    public static float NARAKU_HEIGHT = -5.0f;

    // LevelControl�� �����ϱ� ���� �ʿ�
    public float current_speed = 0.0f; // ���� �ӵ�
    public LevelControl level_control = null; // LevelControl�� ����

    private float click_timer = 1.0f; // ��ư�� ���� ���� �ð�
    private float CLICK_GRACE_TIME = 0.5f; // �����ϰ� ���� �ǻ縦 �޾Ƶ��� �ð�

    void Start()
    {
        this.next_step = STEP.RUN;
    }

    void Update()
    {
        Vector3 velocity = this.GetComponent<Rigidbody>().velocity; // �ӵ��� ����

        // �Ʒ� ���� �ӵ��� �������� �޼��� ȣ�� �߰�
        this.current_speed = this.level_control.getPlayerSpeed();

        this.check_landed(); // ���� �������� üũ

        switch (this.step)
        {
            case STEP.RUN:
            case STEP.JUMP:
                // ���� ��ġ�� �Ѱ�ġ���� �Ʒ���,
                if (this.transform.position.y < NARAKU_HEIGHT)
                {
                    this.next_step = STEP.MISS; // '����' ����
                }
                break;
        }

        this.step_timer += Time.deltaTime; // ��� �ð��� ����
                                           // ���� ���°� ������ ���� ������ ������ ��ȭ�� ����
        if (Input.GetMouseButtonDown(0))
        { // ��ư�� ��������,
            this.click_timer = 0.0f; // Ÿ�̸Ӹ� ����
        }
        else
        {
            if (this.click_timer >= 0.0f)
            { // �׷��� ������,
                this.click_timer += Time.deltaTime; // ��� �ð��� ����
            }
        }

        if (this.next_step == STEP.NONE)
        {
            switch (this.step)
            { // Player�� ���� ���·� �б�
                case STEP.RUN: // �޸��� ���� ��
                               //if (!this.is_landed)
                               //{
                               //    // �޸��� ���̰� �������� ���� ��� �ƹ��͵� ���� ����
                               //}
                               //else
                               //{
                               //    if (Input.GetMouseButtonDown(0))
                               //    {
                               //        // �޸��� ���̰� �����߰� ���� ��ư�� ���ȴٸ�,
                               //        // ���� ���¸� ������ ����
                               //        this.next_step = STEP.JUMP;
                               //    }
                               //}
                               // �Ʒ� �κ����� ����
                    if (0.0f <= this.click_timer && this.click_timer <= CLICK_GRACE_TIME)
                    { // click_timer�� 0�̻�, CLICK_GRACE_TIME���϶��,
                        if (this.is_landed)
                        { // �����ߴٸ�,
                            this.click_timer = -1.0f; // ��ư�� �������� ������ ��Ÿ���� -1.0f��
                            this.next_step = STEP.JUMP; // ���� ���·�
                        }
                    }

                    break;
                case STEP.JUMP: // ���� ���� ��,
                    if (this.is_landed)
                    {
                        // ���� ���̰� �����ߴٸ�, ���� ���¸� ���� ������ ����
                        this.next_step = STEP.RUN;
                    }
                    break;
            }
        }

        // '���� ����'�� '���� ���� ����'�� �ƴ� ����(���°� ���� ����)
        while (this.next_step != STEP.NONE)
        {
            this.step = this.next_step; // '���� ����'�� '���� ����'�� ����
            this.next_step = STEP.NONE; // '���� ����'�� '���� ����'���� ����
            switch (this.step)
            { // ���ŵ� '���� ����'��
                case STEP.JUMP: // '�������� ��,
                                // �ְ� ������ ����(JUMP_HEIGHT_MAX)���� ������ �� �ִ� �ӵ��� ���
                    velocity.y = Mathf.Sqrt(2.0f * 9.8f * PlayerControl.JUMP_HEIGHT_MAX);
                    // '��ư�� ���������� ��Ÿ���� �÷���'�� Ŭ����
                    this.is_key_released = false;
                    break;
            }
            // ���°� �������Ƿ� ��� �ð��� ���η� ����
            this.step_timer = 0.0f;
        }

        // ���º��� �� ������ ���� ó��
        switch (this.step)
        {
            case STEP.RUN: // �޸��� ���� ��,
                           // �ӵ��� ����
                velocity.x += PlayerControl.ACCELERATION * Time.deltaTime;
                // �ӵ��� �ְ� �ӵ� ������ ������,
                //if (Mathf.Abs(velocity.x) > PlayerControl.SPEED_MAX)
                //{
                //    // �ְ� �ӵ� ���� ���Ϸ� ����
                //    velocity.x *= PlayerControl.SPEED_MAX / Mathf.Abs(this.GetComponent<Rigidbody>().velocity.x);
                //}
                if (Mathf.Abs(velocity.x) > this.current_speed)  // ������� ���� �ӵ��� �����ؾ� �� �ӵ��� ������,
                {
                    velocity.x *= this.current_speed / Mathf.Abs(velocity.x); // ���� �ʰ� ����
                }
                break;

            case STEP.JUMP: // ���� ���� ��,
                do
                {
                    // '��ư�� ������ ����'�� �ƴϸ�,
                    if (!Input.GetMouseButtonUp(0))
                    {
                        break; // �ƹ��͵� ���� �ʰ� ������ ��������
                    }
                    // �̹� ���ӵ� ���¸�(�� ���̻� �������� �ʵ���),
                    if (this.is_key_released)
                    {
                        break; // �ƹ��͵� ���� �ʰ� ������ ��������
                    }
                    // ���Ϲ��� �ӵ��� 0 ���ϸ�(�ϰ� ���̶��),
                    if (velocity.y <= 0.0f)
                    {
                        break; // �ƹ��͵� ���� �ʰ� ������ ��������
                    }
                    // ��ư�� ������ �ְ� ��� ���̶��, ���� ����
                    // ������ ����� ���⼭ ��
                    velocity.y *= JUMP_KEY_RELEASE_REDUCE;
                    this.is_key_released = true;
                } while (false);
                break;

            case STEP.MISS:
                // ���ӵ�(ACCELERATION)�� ���� Player�� �ӵ��� ������
                velocity.x -= PlayerControl.ACCELERATION * Time.deltaTime;
                if (velocity.x < 0.0f)
                { // Player�� �ӵ��� ���̳ʽ���,
                    velocity.x = 0.0f; // 0����
                }
                break;
        }
        // Rigidbody�� �ӵ��� ������ ���� �ӵ��� ���� (�� ���� ���¿� ������� �Ź� ����)
        this.GetComponent<Rigidbody>().velocity = velocity;
    }

    private void check_landed()
    { // �����ߴ��� ����
        this.is_landed = false; // �ϴ� false�� ����
        do
        {
            Vector3 s = this.transform.position; // Player�� ���� ��ġ
            Vector3 e = s + Vector3.down * 1.0f; // s���� �Ʒ��� 1�� �̵��� ��ġ
            RaycastHit hit;
            // s���� e ���̿� �ƹ��͵� ���� ��
            // out: method ������ ������ ���� ��ȯ �� ���
            if (!Physics.Linecast(s, e, out hit))
            {
                break; // �ƹ��͵� ���� �ʰ� do~while ������ ��������(Ż�ⱸ��)
            }
            // s���� e ���̿� ���� ���� �� �Ʒ��� ó���� ����
            if (this.step == STEP.JUMP)
            { // ����, ���� ���¶��
                if (this.step_timer < Time.deltaTime * 3.0f)
                { // ��� �ð��� 3.0f �̸��̶��
                    break; // �ƹ��͵� ���� �ʰ� do~while ������ ��������(Ż�ⱸ��)
                }
            }
            // s���� e ���̿� ���� �ְ� JUMP ���İ� �ƴ� ���� �Ʒ��� ����
            this.is_landed = true;
        } while (false);
        // ������ Ż�ⱸ
    }

    // ���� ���Ḧ ���� �Ʒ� �Լ� �߰�
    public bool isPlayEnd()
    { // ������ �������� ����
        bool ret = false;
        switch (this.step)
        {
            case STEP.MISS: // MISS ���¶��,
                ret = true; // '�׾����'(true)��� �˷���
                break;
        }
        return (ret);
    }
}