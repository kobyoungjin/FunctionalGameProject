using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    public static float MOVE_AREA_RADIUS = 15.0f; // ���� ������.
    public static float MOVE_SPEED = 7.0f; // �̵� �ӵ�. 

    private struct Key
    { // Ű ���� ���� ����ü.
        public bool up; // ��.
        public bool down; // ��.
        public bool right; // ��.
        public bool left; // ��.
        public bool pick; // �ݴ´٣�������.
        public bool action; // �Դ´� / �����Ѵ�.
    };

    private Key key; // Ű ���� ������ �����ϴ� ����.

    public enum STEP
    { // �÷��̾��� ���¸� ��Ÿ���� ����ü.
        NONE = -1, // ���� ���� ����.
        MOVE = 0, // �̵� ��.
        REPAIRING, // ���� ��.
        EATING, // �Ļ� ��.
        NUM, // ���°� �� ���� �ִ��� ��Ÿ����(=3).
    };

    public STEP step = STEP.NONE; // ���� ����.
    public STEP next_step = STEP.NONE; // ���� ����.
    public float step_timer = 0.0f; // Ÿ�̸�.

    private GameObject closest_item = null; // �÷��̾��� ���鿡 �ִ� GameObject.
    private GameObject carried_item = null; // �÷��̾ ���ø� GameObject.
    private ItemRoot item_root = null; // ItemRoot ��ũ��Ʈ�� ����.
    public GUIStyle guistyle; // ��Ʈ ��Ÿ��.

    private GameObject closest_event = null;// �ָ��ϰ� �ִ� �̺�Ʈ�� ����.
    private EventRoot event_root = null;    // EventRoot Ŭ������ ����ϱ� ���� ����.
    private GameObject rocket_model = null; // ���ּ��� ���� ����ϱ� ���� ����.

    private GameStatus game_status = null;

    // Use this for initialization
    void Start()
    {
        this.step = STEP.NONE; // �� �ܰ� ���¸� �ʱ�ȭ.
        this.next_step = STEP.MOVE; // ���� �ܰ� ���¸� �ʱ�ȭ.	

        this.item_root = GameObject.Find("GameRoot").GetComponent<ItemRoot>();
        this.guistyle.fontSize = 16;

        this.event_root = GameObject.Find("GameRoot").GetComponent<EventRoot>();
        this.rocket_model = GameObject.Find("rocket").transform.Find("rocket_model").gameObject;
        this.game_status = GameObject.Find("GameRoot").GetComponent<GameStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        this.get_input(); // �Է� ���� ���. 

        this.step_timer += Time.deltaTime;
        float eat_time = 0.5f; // ����� 2�ʿ� ���� �Դ´�.
        float repair_time = 0.5f; // ������ �ɸ��� �ð��� 2��.

        // ���¸� ��ȭ��Ų��---------------------.
        if (this.next_step == STEP.NONE)
        { // ���� ������ ������.
            switch (this.step)
            {
                case STEP.MOVE: // '�̵� ��' ������ ó��.
                    do
                    {
                        if (!this.key.action)
                        { // �׼� Ű�� �������� �ʴ�.
                            break; // ���� Ż��.
                        }

                        // �ָ��ϴ� �̺�Ʈ�� ���� ��.
                        if (this.closest_event != null)
                        {
                            if (!this.is_event_ignitable())
                            { // �̺�Ʈ�� ������ �� ������.
                                break; // �ƹ� �͵� ���� �ʴ´�.
                            }
                            // �̺�Ʈ ������ �����´�.
                            Event.TYPE ignitable_event =
                                this.event_root.getEventType(this.closest_event);
                            switch (ignitable_event)
                            {
                                case Event.TYPE.ROCKET:
                                    // �̺�Ʈ�� ������ ROCKET�̸�.
                                    // REPAIRING(����) ���·� ����.
                                    this.next_step = STEP.REPAIRING;
                                    break;
                            }
                            break;
                        }

                        if (this.carried_item != null)
                        {
                            // ������ �ִ� ������ �Ǻ�.
                            Item.TYPE carried_item_type =
                                this.item_root.getItemType(this.carried_item);

                            switch (carried_item_type)
                            {
                                case Item.TYPE.APPLE: // ������.
                                case Item.TYPE.PLANT: // �Ĺ��̶��.
                                    // '�Ļ� ��' ���·� ����.
                                    this.next_step = STEP.EATING;
                                    break;
                            }
                        }
                    } while (false);
                    break;

                case STEP.EATING: // '�Ļ� ��' ������ ó��.
                    if (this.step_timer > eat_time)
                    { // 2�� ���.
                        this.next_step = STEP.MOVE; // '�̵�' ���·� ����.
                    }
                    break;

                case STEP.REPAIRING: // '���� ��' ������ ó��.
                    if (this.step_timer > repair_time)
                    { // 2�� ���.
                        this.next_step = STEP.MOVE; // '�̵�' ���·� ����.
                    }
                    break;
            }
        }

        // ���°� ��ȭ���� ��------------.
        while (this.next_step != STEP.NONE)
        { // ���°� NONE�̿� = ���°� ��ȭ�ߴ�.
            this.step = this.next_step;
            this.next_step = STEP.NONE;
            switch (this.step)
            {
                case STEP.MOVE:
                    break;

                case STEP.EATING: // '�Ļ� ��' ������ ó��.
                    if (this.carried_item != null)
                    {
                        // ��� �ִ� �������� 'ü�� ȸ�� ����'�� �����ͼ� ����.
                        this.game_status.addSatiety(this.item_root.getRegainSatiety(this.carried_item));

                        // ������ �ִ� �������� ���.
                        GameObject.Destroy(this.carried_item);
                        this.carried_item = null;
                    }
                    break;

                case STEP.REPAIRING: // '���� ��'�� �Ǹ�.
                    if (this.carried_item != null)
                    {
                        // ��� �ִ� �������� '���� ��ô ����'�� �����ͼ� ����.
                        this.game_status.addRepairment(this.item_root.getGainRepairment(this.carried_item));

                        // ������ �ִ� ������ ����.
                        GameObject.Destroy(this.carried_item);
                        this.carried_item = null;
                        this.closest_item = null;
                    }
                    break;
            }
            this.step_timer = 0.0f;
        }

        // �� ��Ȳ���� �ݺ��� ��----------.
        switch (this.step)
        {
            case STEP.MOVE:
                this.move_control();
                this.pick_or_drop_control(); 
                //this.game_status.alwaysSatiety();  // �̵� ������ ���� �׻� �谡 ��������.
                break;
            case STEP.REPAIRING:
                // ���ּ��� ȸ����Ų��.
                this.rocket_model.transform.localRotation *= Quaternion.AngleAxis(360.0f / 10.0f * Time.deltaTime, Vector3.up);
                break;
        }
    }

    private void get_input()
    {
        this.key.up = false;
        this.key.down = false;
        this.key.right = false;
        this.key.left = false;
        // ��Ű�� �������� true�� ����.
        this.key.up |= Input.GetKey(KeyCode.UpArrow);
        this.key.up |= Input.GetKey(KeyCode.Keypad8);
        // ��Ű�� �������� true�� ����.
        this.key.down |= Input.GetKey(KeyCode.DownArrow);
        this.key.down |= Input.GetKey(KeyCode.Keypad2);
        // ��Ű�� �������� true�� ����.
        this.key.right |= Input.GetKey(KeyCode.RightArrow);
        this.key.right |= Input.GetKey(KeyCode.Keypad6);
        // ��Ű�� �������� true�� ����..
        this.key.left |= Input.GetKey(KeyCode.LeftArrow);
        this.key.left |= Input.GetKey(KeyCode.Keypad4);
        // Z Ű�� �������� true�� ����.
        this.key.pick = Input.GetKeyDown(KeyCode.Z);
        // X Ű�� �������� true�� ����.
        this.key.action = Input.GetKeyDown(KeyCode.X);
    }

    private void move_control()
    {
        Vector3 move_vector = Vector3.zero; // �̵��� ����.
        Vector3 position = this.transform.position; // ���� ��ġ�� ����.
        bool is_moved = false;

        if (this.key.right)
        { // ��Ű�� ��������.
            move_vector += Vector3.right; // �̵��� ���͸� ���������� ���Ѵ�.
            is_moved = true; // '�̵� ��' �÷���. 
        }

        if (this.key.left)
        {
            move_vector += Vector3.left;
            is_moved = true;
        }

        if (this.key.up)
        {
            move_vector += Vector3.forward;
            is_moved = true;
        }

        if (this.key.down)
        {
            move_vector += Vector3.back;
            is_moved = true;
        }

        move_vector.Normalize(); // ���̸� 1��.
        move_vector *= MOVE_SPEED * Time.deltaTime; // �ӵ����ð����Ÿ�.
        position += move_vector; // ��ġ�� �̵�.
        position.y = 0.0f; // ���̸� 0���� �Ѵ�.

        // ������ �߾ӿ��� ������ ��ġ������ �Ÿ��� ���� ���������� ũ��.
        if (position.magnitude > MOVE_AREA_RADIUS)
        {
            position.Normalize();
            position *= MOVE_AREA_RADIUS; // ��ġ�� ���� ���ڶ��� �ӹ��� �Ѵ�.
        }

        // ���� ���� ��ġ(position)�� ���̸� ���� ���̷� �ǵ�����.
        position.y = this.transform.position.y;
        // ���� ��ġ�� ���� ���� ��ġ�� �����Ѵ�.
        this.transform.position = position;
        // �̵� ������ ���̰� 0.01���� ū ���.
        // =��� ���� �̻��� �̵��� ���.

        if (move_vector.magnitude > 0.01f)
        {
            // ĳ������ ������ õõ�� �ٲ۴�.
            Quaternion q = Quaternion.LookRotation(move_vector, Vector3.up);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, q, 0.2f);
        }

        if (is_moved)
        {
            // ��� �ִ� �����ۿ� ���� 'ü�� �Ҹ� ����'�� �����Ѵ�.
            float consume = this.item_root.getConsumeSatiety(this.carried_item);
            // ������ '�Ҹ� ����'�� ü�¿��� ����.
            this.game_status.addSatiety(-consume * Time.deltaTime);
        }
    }

    void OnTriggerStay(Collider other)
    {
        GameObject other_go = other.gameObject;


        // Ʈ������ GameObject ���̾� ������ Item�̶��.
        if (other_go.layer == LayerMask.NameToLayer("Item"))
        {
            // �ƹ� �͵� �ָ��ϰ� ���� ������.
            if (this.closest_item == null)
            {
                if (this.is_other_in_view(other_go))
                { // ���鿡 ������.
                    this.closest_item = other_go; // �ָ��Ѵ�.
                }
                // ���� �ָ��ϰ� ������.
            }
            else if (this.closest_item == other_go)
            {
                if (!this.is_other_in_view(other_go))
                { // ���鿡 ������.
                    this.closest_item = null; // �ָ��� �׸��д�.
                }
            }
        }
        // Ʈ������ GameObject�� ���̾� ������ Event���.
        else if (other_go.layer == LayerMask.NameToLayer("Event"))
        {
            // �ƹ��͵� �ָ��ϰ� ���� ������.
            if (this.closest_event == null)
            {
                if (this.is_other_in_view(other_go))
                {  // ���鿡 ������.
                    this.closest_event = other_go;      // �ָ��Ѵ�.
                }
                // ������ �ָ��ϰ� ������.
            }
            else if (this.closest_event == other_go)
            {
                if (!this.is_other_in_view(other_go))
                { // ���鿡 ������.
                    this.closest_event = null;          // �ָ��� �׸��д�.
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (this.closest_item == other.gameObject)
        {
            this.closest_item = null; // �ָ��� �׸��д�.
        }
    }
    /*
        void OnGUI()
        {
            float x = 20.0f;
            float y = Screen.height - 40.0f;
            // ��� �ִ� �������� �ִٸ�.
            if (this.carried_item != null)
            {
                GUI.Label(new Rect(x, y, 200.0f, 20.0f), "Z:������", guistyle);
                GUI.Label(new Rect(x + 100.0f, y, 200.0f, 20.0f),
                          "X:�Դ´�", guistyle);
            }
            else
            {
                // �ָ��ϰ� �ִ� �������� �ִٸ�.
                if (this.closest_item != null)
                {
                    GUI.Label(new Rect(x, y, 200.0f, 20.0f), "Z:�ݴ´�", guistyle);
                }
            }

            switch (this.step)
            {
                case STEP.EATING:
                    GUI.Label(new Rect(x, y, 200.0f, 20.0f),
                              "���������칰�칰����", guistyle);
                    break;
            }
        }
    */


    private void pick_or_drop_control()
    {
        do
        {
            if (!this.key.pick)
            { // '�ݱ�/������'Ű�� ������ �ʾ�����.
                break; // �ƹ��͵� ���� �ʰ� �޼ҵ� ����.
            }
            if (this.carried_item == null)
            { // ��� �ִ� �������� ����.
                if (this.closest_item == null)
                { // �ָ� ���� �������� ������.
                    break; // �ƹ��͵� ���� �ʰ� �޼ҵ� ����.
                }
                // �ָ� ���� �������� ���ø���.
                this.carried_item = this.closest_item;
                // ��� �ִ� �������� �ڽ��� �ڽ����� ����.
                this.carried_item.transform.parent = this.transform;
                // 2.0f ���� ��ġ(�Ӹ� ���� �̵�).
                this.carried_item.transform.localPosition = Vector3.up * 2.0f;
                // �ָ� �� �������� ���ش�.
                this.closest_item = null;
            }
            else
            { // ��� �ִ� �������� ���� ���.
                // ��� �ִ� �������� �ణ(1.0f) ������ �̵����Ѽ�.
                this.carried_item.transform.localPosition =
                    Vector3.forward * 1.0f;
                this.carried_item.transform.parent = null; // �ڽ� ������ ����.
                this.carried_item = null; // ��� �ִ� �������� ���ش�.
            }
        } while (false);
    }

    private bool is_other_in_view(GameObject other)
    {
        bool ret = false;
        do
        {
            Vector3 heading = // �ڽ��� ���� ���ϰ� �ִ� ������ ����.
                this.transform.TransformDirection(Vector3.forward);
            Vector3 to_other = // �ڽ� �ʿ��� �� �������� ������ ����.
                other.transform.position - this.transform.position;
            heading.y = 0.0f;
            to_other.y = 0.0f;
            heading.Normalize(); // ���̸� 1�� �ϰ� ���⸸ ���ͷ�.
            to_other.Normalize(); // ���̸� 1�� �ϰ� ���⸸ ���ͷ�.
            float dp = Vector3.Dot(heading, to_other); // ���� ������ ������ ���.
            if (dp < Mathf.Cos(45.0f))
            { // ������ 45���� �ڻ��� �� �̸��̸�.
                break; // ������ ����������.
            }
            ret = true; // ������ 45���� �ڻ��� �� �̻��̸� ���鿡 �ִ�.
        } while (false);
        return (ret);
    }

    private bool is_event_ignitable()
    {
        bool ret = false;
        do
        {
            if (this.closest_event == null)
            { // �ָ� �̺�Ʈ�� ������.
                break; // false�� ��ȯ�Ѵ�.
            }

            // ��� �ִ� ������ ������ �����´�.
            Item.TYPE carried_item_type =
                this.item_root.getItemType(this.carried_item);

            // ��� �ִ� ������ ������ �ָ��ϴ� �̺�Ʈ�� ��������.
            // �̺�Ʈ�� �������� �����ϰ�, �̺�Ʈ �Ұ���� false�� ��ȯ�Ѵ�.
            if (!this.event_root.isEventIgnitable(carried_item_type, this.closest_event))
            {
                break;
            }
            ret = true; // ������� ���� �̺�Ʈ�� ������ �� �ִٰ� ����!.
        } while (false);
        return (ret);
    }

    void OnGUI()
    {
        float x = 20.0f;
        float y = Screen.height - 40.0f;

        if (this.carried_item != null)
        {
            GUI.Label(new Rect(x, y, 200.0f, 20.0f), "Z:������", guistyle);
            do
            {
                if (this.is_event_ignitable())
                {
                    break;
                }
                if (item_root.getItemType(this.carried_item) == Item.TYPE.ROCK)
                {
                    break;
                }
                GUI.Label(new Rect(x + 100.0f, y, 200.0f, 20.0f), "x:�Դ´�", guistyle);
            } while (false);
        }
        else
        {
            if (this.closest_item != null)
            {
                GUI.Label(new Rect(x, y, 200.0f, 20.0f), "Z:�ݴ´�", guistyle);
            }
        }

        switch (this.step)
        {
            case STEP.EATING:
                GUI.Label(new Rect(Screen.width / 2 - 80.0f, y, 200.0f, 20.0f), "��ƿ�ƿ칰�칰����", guistyle);
                break;
            case STEP.REPAIRING:
                GUI.Label(new Rect(Screen.width / 2 - 20.0f, y, 200.0f, 20.0f), "������", guistyle);
                break;
        }
        Debug.Log(this.is_event_ignitable());

        if (this.is_event_ignitable())
        { // �̺�Ʈ�� ���� ������ ���.
            // �̺�Ʈ�� �޽����� ���.
            string message = this.event_root.getIgnitableMessage(this.closest_event);
            GUI.Label(new Rect(x + 200.0f, y, 200.0f, 20.0f), "X:" + message, guistyle);
        }
    }

}
