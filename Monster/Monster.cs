using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour
{
    public enum MonsterState
    {
        Idle,
        Move,
        Attack,
        TakeHit,
        Return,
        Die,
    }

    NavMeshAgent agent;
    Animator animator;
    public Transform target;
    new Rigidbody rigidbody;

    public int monsterID;
    public MonsterState monsterState = MonsterState.Idle;
    public bool isStartCoroutine = true;

    [SerializeField]
    Transform attackTransform;

    Vector3 firstPos;
    public float originalSpeed;
    int MaxHP = 100;
    public int HP;
    public int monsterExp;
    public int monsterGold;

    public AudioSource monsterHitSound;
    //public GameObject monsterHitSoundGO;
    public GameObject monsterWalkSoundGO;
    public HPBar hpBar;

    public bool firstMake = false;

    public IEnumerator Start()
    {
        firstMake = true;
        firstPos = transform.position;
        HP = MaxHP;
        agent = GetComponent<NavMeshAgent>();
        originalSpeed = agent.speed;
        animator = GetComponentInChildren<Animator>();
        attackTransform = transform.Find("AttackPosition").transform.GetComponent<Transform>();
        hpBar = transform.Find("HPBar/Canvas").GetComponent<HPBar>();

        monsterHitSound = transform.Find("Sounds").GetChild(0).GetComponent<AudioSource>();

        rigidbody = transform.GetComponent<Rigidbody>();

        target = Player.Instance.transform;
        MonsterInitialization();

        hpBar.UpdateMonsterHPBar(HP, MaxHP);

        CurrentFsm = IdleFSM;
        while (true)
        {
            var previousFSM = CurrentFsm;
            fsmHandle = StartCoroutine(CurrentFsm());

            if (fsmHandle == null && previousFSM == CurrentFsm)
            {
                yield return null;
            }
            while (fsmHandle != null)
            {
                yield return null;
            }
        }
    }

    private void OnEnable()
    {
        if(firstMake)
            StartCoroutine(Start());
    }

    private void MonsterInitialization()
    {
        var monsterInfo = MySQL.instance.monsters.Find(x => x.monsterID == monsterID);
        MaxHP = monsterInfo.monsterHP;
        HP = MaxHP;
        monsterDmg = monsterInfo.monsterDmg;
        monsterGold = monsterInfo.monsterGold;
        monsterExp = monsterInfo.monsterExp;
    }

    internal void TakeHit(int playerDamage)
    {
        if(fsmHandle != null)
            StopCoroutine(fsmHandle);
        agent.speed = 0;
        //agent;
        if (monsterState == MonsterState.Die)
            // 죽는 소리 추가
            return;
        HP -= playerDamage;
        hpBar.UpdateMonsterHPBar(HP, MaxHP);
        // 맞는 소리 추가
        monsterHitSound.volume = 0.4f;
        monsterHitSound.PlayOneShot(monsterHitSound.clip);

        DamageTextPrint(playerDamage);
        if (HP <= 0)
            CurrentFsm = DieFSM;
        else
            CurrentFsm = TakeHitFSM;
    }
    public float dieAnimTime = 1f;
    private GameObject coin;
    private IEnumerator DieFSM()
    {
        animator.Play("Die");
        yield return new WaitForSeconds(dieAnimTime);

        InstantiateCoin();

        monsterHitSound.volume = 0f;

        UserDB.instance.userExp += monsterExp;

        CheckQuest();
        MySQL.instance.UpdateUserData();
        MonsterInfoInit();
    }

    private void CheckQuest()
    {
        if (UserDB.instance.userAcceptQuests.Count != 0)
        {
            for (int i = 0; i < UserDB.instance.userAcceptQuests.Count; i++)
            {
                if (UserDB.instance.userAcceptQuests[i].questTargetID == monsterID &&
                    UserDB.instance.userAcceptQuests[i].questTotalGoal > UserDB.instance.userAcceptQuests[i].questAmount)
                {
                    UpdateQuest(i);
                }
            }
        }
    }

    private void UpdateQuest(int i)
    {
        UserDB.instance.userAcceptQuests[i].questAmount += 1;
        // 퀘스트가 완료 되었다면 해당 isQuestClear를 true로 변경해주자
        if (UserDB.instance.userAcceptQuests[i].questAmount == UserDB.instance.userAcceptQuests[i].questTotalGoal)
            UserDB.instance.userAcceptQuests[i].isQuestClear = true;
        for (int j = 0; j < UserDB.instance.userAcceptQuests.Count; j++)
        {
            if (QuickQuestUI.instance.quickQuestBaseItems[j].quickQuestBaseItemQuestDB.questID == UserDB.instance.userAcceptQuests[i].questID)
            {
                QuickQuestUI.instance.quickQuestBaseItems[j].UpdateQuestGoal(UserDB.instance.userAcceptQuests[i]);
            }
        }
    }

    private void MonsterInfoInit()
    {
        transform.position = firstPos;
        CurrentFsm = IdleFSM;
        HP = MaxHP;
        hpBar.UpdateMonsterHPBar(HP, MaxHP);
        if (agent.enabled == false)
            agent.enabled = true;
        agent.speed = originalSpeed;
        isStartCoroutine = false;
        gameObject.SetActive(false);
    }

    private void InstantiateCoin()
    {
        coin = CoinObjectPool.instance.coins[CoinObjectPool.instance.coins.Count - 1];
        CoinObjectPool.instance.coins.RemoveAt(CoinObjectPool.instance.coins.Count - 1);
        coin.transform.parent = null;
        coin.transform.position = transform.position;
        coin.GetComponent<Coin>().coinPoint = monsterGold;
        coin.SetActive(true);
    }

    public float takeHitAnimTime = 1f;
    public float takeHitPower = 3f;
    private IEnumerator TakeHitFSM()
    {
        animator.Play("Damage",0,0);

        agent.enabled = false;

        var direction = transform.position - Player.Instance.transform.position;
        rigidbody.isKinematic = false;
        rigidbody.AddForce(direction.normalized * takeHitPower, ForceMode.Impulse);
        yield return new WaitForSeconds(takeHitAnimTime);

        rigidbody.isKinematic = true;
        agent.enabled = true;
        CurrentFsm = ChaseFSM;
    }

    private IEnumerator ReturnBackFSM()
    {
        while (Vector3.Distance(transform.position, firstPos) > 1)
        {
            agent.destination = firstPos;
            yield return null;
        }
        CurrentFsm = IdleFSM;
    }

    public float monsterAttackDistance =3;
    public float monsterChaseDistance = 5;
    private IEnumerator IdleFSM()
    {
        monsterState = MonsterState.Idle;
        animator.Play("Idle");
        while (Vector3.Distance(target.position, transform.position) > monsterChaseDistance)
        {
            yield return null;
        }
        CurrentFsm = ChaseFSM;
    }

    Coroutine fsmHandle;

    Func<IEnumerator> m_currentFsm;
    public Func<IEnumerator> CurrentFsm
    {
        get { return m_currentFsm; }
        set
        {
            if (fsmHandle != null)
            {
                StopCoroutine(fsmHandle);   // 새로운 코루틴 함수가 실행되면 그전의 함수는 종료가 되야됨
            }

            m_currentFsm = value;
            fsmHandle = null;
        }
    }

    private IEnumerator ChaseFSM()
    {
        agent.speed = originalSpeed;
        animator.Play("Move");
        yield return new WaitForSeconds(Random.Range(1, 1.5f));
        
        monsterState = MonsterState.Move;
        while (Vector3.Distance(target.position, transform.position) > monsterAttackDistance)
        {
            agent.destination = target.position;
            if (Vector3.Distance(transform.position, firstPos) > returnBackDistance)
            {
                monsterState = MonsterState.Return;
                yield return null;
                break;
            }
            yield return null;
        }
        if(monsterState == MonsterState.Return)
        {
            CurrentFsm = ReturnBackFSM;
        }
        else
        {
            CurrentFsm = AttackFSM;
        }
    }

    public float monsterAttackStartTime = 0.3f;
    public float monsterAttackTime = 1;
    public int monsterDmg = 10;
    private IEnumerator AttackFSM()
    {
        monsterState = MonsterState.Attack;
        agent.speed = 0;
        yield return new WaitForSeconds(monsterAttackStartTime);
        animator.Play("Attack");
        yield return new WaitForSeconds(monsterAttackStartTime);
        var colliders = Physics.OverlapSphere(attackTransform.position, 0.75f);
        foreach (var item in colliders)
        {
            if (item.CompareTag("Player"))
            {
                // 플레이어에게 데미지를 주자
                Player.Instance.TakeHit(monsterDmg);
            }
        }
        yield return new WaitForSeconds(monsterAttackTime - monsterAttackStartTime);
        CurrentFsm = ChaseFSM;
    }
    public float returnBackDistance = 20f;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, monsterChaseDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, monsterAttackDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackTransform.position, 0.75f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(firstPos, returnBackDistance);
    }

    private void DamageTextPrint(int damage)
    {
        var damageText = Instantiate((GameObject)Resources.Load("DamageText"), transform);
        damageText.transform.LookAt(Camera.main.transform);
        damageText.transform.Rotate(0, 180, 0);
        damageText.GetComponent<TextMeshPro>().text = damage.ToString();
    }
}
