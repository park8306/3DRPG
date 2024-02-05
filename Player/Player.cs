using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    public static Player Instance;
    [SerializeField]
    private Transform characterBody;
    [SerializeField]
    private Transform cameraArm;

    public GameObject SwordParticleEffectGO;
    public GameObject SkillEffectGO;
    public GameObject FootSoundGO;
    public GameObject SkillCoolTimeGO;
    public GameObject LevelUpUIGO;

    public AudioSource swordSound;

    Animator animator;

    public int playerDamage = 0;
    public int playerArmor = 0;
    public int MaxHP = 50;
    public int HP;

    public Transform AttackTr;
    public AnimationCurve rollingSpeedAC;
    public int playerLevel;
    public enum PlayerState
    {
        IdleOrWalk,
        Attack1,
        Attack2,
        Roll,
        TakeHit,
        Die,
        Skill,
    }

    private void Awake()
    {
        Instance = this;
        AttackTr = transform.Find("Mesh/AttackCollider").GetComponent<Transform>();
        MP = MaxMP;
    }
    public PlayerState playerState = PlayerState.IdleOrWalk;

    public Vector3 cameraOffset;
    // Start is called before the first frame update
    public float skillAbleTime = 5;
    GameObject skillColliderObject;
    void Start()
    {
        playerLevel = UserDB.instance.userLv;
        playerDamage += playerLevel * 10;
        playerArmor += playerLevel * 5;
        MaxHP += playerLevel * 50;
        animator = GetComponentInChildren<Animator>();
        cameraOffset = characterBody.position - cameraArm.position;
        HP = MaxHP;
        SwordParticleEffectGO.SetActive(false);
        SkillEffectGO.SetActive(false);
        FootSoundGO = transform.Find("FootSound").gameObject;
        FootSoundGO.SetActive(false);
        SkillCoolTimeGO = GameObject.Find("SkillCoolTime").gameObject;
        LevelUpUIGO = transform.Find("Canvas/LevelUp_Badge").gameObject;

        LevelUpUIGO.SetActive(false);

        StartCoroutine(FillMPCo());
    }

    // Update is called once per frame
    void Update()
    {
        
        FollowAndLookAround();
        Move();
        Roll();
        Attack();
    }

    private void Roll()
    {
        if(Input.GetKeyDown(KeyCode.Space) && playerState == PlayerState.IdleOrWalk && MP > 0)
        {
            MP -= 20;
            StartCoroutine(RollCo());
        }
    }

    Coroutine fillMpHandle;
    private IEnumerator FillMPCo()
    {
        fillMpHandle = StartCoroutine(FillFinishCo());
        while (true)
        {
            if (MP < MaxMP && playerState == PlayerState.IdleOrWalk)
            {
                yield return new WaitForSeconds(1f);
                fillMpHandle = StartCoroutine(FillMPNextCo());
            }
            else if (MP >= MaxMP && playerState != PlayerState.IdleOrWalk)
            {
                StopCoroutine(fillMpHandle);
                fillMpHandle = StartCoroutine(FillFinishCo());
            }
            yield return null;
        }
    }

    private IEnumerator FillFinishCo()
    {
        yield return null;
    }

    private IEnumerator FillMPNextCo()
    {
        yield return null;
        MP += 10;
    }

    public float rollDistance = 1f;
    public float speed = 1f;
    public float rollingSpeedUserMultipy = 1f;
    public int MaxMP = 100;
    public int MP;
    private IEnumerator RollCo()
    {
        animator.SetFloat("Walk", 0);
        playerState = PlayerState.Roll;
        animator.Play("Roll");
        transform.GetComponent<Collider>().enabled = false;
        float starTime = Time.time;
        float endTime = starTime + rollingSpeedAC[rollingSpeedAC.length - 1].time;
        while (endTime > Time.time)
        {
            float time = Time.time - starTime;
            float rollingSpeedMultipy = rollingSpeedAC.Evaluate(time) * rollingSpeedUserMultipy;

            transform.Translate(characterBody.forward * speed * rollingSpeedMultipy * Time.deltaTime, Space.World);
            yield return null;
            if(endTime - Time.time <= 0.2f && transform.GetComponent<Collider>().enabled == false)
            {
                transform.GetComponent<Collider>().enabled = true;
            }
        }
        playerState = PlayerState.IdleOrWalk;
    }

    public float mouseSpeed = 3f;
    private void FollowAndLookAround()
    {
        if (GameManager.gameState != GameState.Play)
            return;
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X") * mouseSpeed, Input.GetAxis("Mouse Y") * mouseSpeed);
        Vector3 camAngle = cameraArm.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;
        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }
        cameraArm.position = characterBody.position - cameraOffset;
        cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }

    [SerializeField]
    float characterSpeed = 5f;

    private void Move()
    {
        if (playerState != PlayerState.IdleOrWalk)
            return;
        playerState = PlayerState.IdleOrWalk;
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMove = moveInput.magnitude != 0;
        if (isMove && playerState == PlayerState.IdleOrWalk)
        {
            FootSoundGO.SetActive(true);
            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            animator.SetFloat("Walk", 1);
            characterBody.forward = Vector3.Lerp(characterBody.forward, moveDir, 0.5f);
            transform.position += moveDir * Time.deltaTime * characterSpeed;
        }
        else
        {
            FootSoundGO.SetActive(false);
            animator.SetFloat("Walk", 0);
        }
    }

    
    internal void TakeHit(int monsterDmg)
    {
        playerState = PlayerState.TakeHit;
        int damage = monsterDmg - (playerArmor/10);
        HP -= damage;
        UserInfoUI.Instance.ChangeHP(HP);
        var damageText = Instantiate((GameObject)Resources.Load("DamageText"), transform);
        damageText.transform.LookAt(Camera.main.transform);
        damageText.transform.Rotate(0, 180, 0);
        damageText.GetComponent<TextMeshPro>().text = damage.ToString();
        if (HP <= 0)
        {
            StartCoroutine(DieCo());
        }
        playerState = PlayerState.IdleOrWalk;
    }

    private IEnumerator DieCo()
    {
        playerState = PlayerState.Die;
        GetComponent<Collider>().enabled = false;
        animator.Play("Die");
        yield return new WaitForSeconds(2f);
        playerState = PlayerState.IdleOrWalk;
        GetComponent<Collider>().enabled = true;
        animator.Play("Idle");
        HP = MaxHP;
        UserInfoUI.Instance.ChangeHP(HP);
    }

    public float continousAttackableTime = 1.5f;
    float attackAbleTime = 0.8f;
    private void Attack()
    {
        if (GameManager.gameState != GameState.Play)
            return;
        attackAbleTime += Time.deltaTime;
        skillAbleTime += Time.deltaTime;
        if (attackAbleTime > 1.5f)
        {
            atkNum = 0;
        }
        if (attackAbleTime > 0.8)
        {
            if (Input.GetMouseButtonDown(0))
            {
                attackAbleTime = 0;
                if (attackHandle != null && playerState != PlayerState.Skill)
                {
                    StopCoroutine(attackHandle);
                }
                attackHandle = StartCoroutine(AttackCo());
            }
        }
        if (skillAbleTime > 5)
        {
            if (Input.GetMouseButtonDown(1))
            {
                skillAbleTime = 0;
                if (attackHandle != null)
                {
                    StopCoroutine(attackHandle);
                }
                SkillCoolTimeGO.GetComponent<SkillCoolTime>().SkillCoolTimeActive();
                attackHandle = StartCoroutine(SkillCo());
            }
        }
    }
    public float skillTime = 1f;
    private IEnumerator SkillCo()
    {
        SkillEffectGO.SetActive(true);
        yield return new WaitForSeconds(skillTime);
        animator.Play("SkillAttack");
        SwordParticleEffectGO.SetActive(true);
        playerState = PlayerState.Skill;
        Collider[] monsters = Physics.OverlapSphere(transform.position, 3f);
        foreach (var item in monsters)
        {
            if (item.CompareTag("Monster"))
            {
                item.GetComponent<Monster>().TakeHit(playerDamage*2);
            }
        }
        yield return new WaitForSeconds(skillTime);
        SwordParticleEffectGO.SetActive(false);
        SkillEffectGO.SetActive(false);
        playerState = PlayerState.IdleOrWalk;
    }

    public float atkNum = 0;
    Coroutine attackHandle;

    public float attackRadius = 1.5f;
    public float hitTime = 0.2f;
    private IEnumerator AttackCo()
    {
        animator.SetFloat("Walk", 0);
        
        playerState = PlayerState.Attack1;
        if(atkNum == 2)
        {
            atkNum = 0;
        }
        AttackAnimation(atkNum);
        atkNum++;
        SwordParticleEffectGO.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        swordSound.PlayOneShot(swordSound.clip);

        yield return new WaitForSeconds(hitTime);
        Collider[] monsters = Physics.OverlapSphere(AttackTr.position, attackRadius);
        foreach (var item in monsters)
        {
            if (item.CompareTag("Monster"))
            {
                item.GetComponent<Monster>().TakeHit(playerDamage);
            }
        }
        yield return new WaitForSeconds(hitTime + attackAbleTime);
        SwordParticleEffectGO.SetActive(false);
        playerState = PlayerState.IdleOrWalk;
    }

    private void AttackAnimation(float atkNum)
    {
        animator.SetFloat("Attack", atkNum);
        animator.SetTrigger("Attack1");
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(AttackTr.position, attackRadius);
        Gizmos.DrawWireSphere(transform.position, 3f);
    }
}