using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Moving variables
    public float m_speed;
    [SerializeField] private Vector3 m_moveDirection;

    //Jump variables
    private Rigidbody2D rb;
    public float m_jumpForce;
    public float m_jumpTimeMax;
    [SerializeField] private float m_jumpTime;
    [SerializeField] private bool isPressJump;
    // [SerializeField] private bool isJumping;

    //Ground check
    public Transform groundCheck;
    public float groundCheckRadius;
    [SerializeField] private bool isTouchingGround;
    public LayerMask groundLayer;
    //Detect attack
    public GameObject slashCollider;
    public GameObject hardSlashCollider;
    public GameObject counterCollider;

    //Animator
    private Animator animator;

    //Button
    private string bLeft;
    private string bRight;
    private string bSlash;
    private string bHardSlash;
    private string bCounter;
    private string bJump;

    //
    public PlayerController enemy;
    public int m_healhPoint;
    private bool isPlayer1;
    private float m_scaleX;
    public GameObject counterText;
    private bool isDamaged;

    //Skill cooldown
    public float m_slashCD;
    public float m_hardSlashCD;
    public float m_counterCD;
    private float m_slashCDPass;
    private float m_hardSlashCDPass;
    private float m_counterCDPass;

    //Audio
    public AudioClip soundAttack;
    public AudioClip soundCounter;
    public AudioClip soundDamage;
    public AudioSource soundSource;

    // Start is called before the first frame update
    void Start()
    {
        m_scaleX = transform.localScale.x;
        if(transform.localScale.x > 0){
            isPlayer1 = true;
        }
        else{
            isPlayer1 = false;
        }
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();

        if(gameObject.name == "Player1"){
            bLeft = "Move Left";
            bRight = "Move Right";
            bSlash = "Slash";
            bHardSlash = "Hard Slash";
            bCounter = "Counter";
            bJump = "Jump";
        }
        else if(gameObject.name == "Player2"){
            bLeft = "Move Left 2";
            bRight = "Move Right 2";
            bSlash = "Slash 2";
            bHardSlash = "Hard Slash 2";
            bCounter = "Counter 2";
            bJump = "Jump 2";
        }
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        Movement();
        ChangeDirection();
    }
    void ChangeDirection(){
        if(isPlayer1){
            if(transform.position.x > enemy.gameObject.transform.position.x && transform.localScale.x>0){
                transform.localScale = new Vector3(-m_scaleX,transform.localScale.y,transform.localScale.z);
            }
            else if(transform.position.x < enemy.gameObject.transform.position.x && transform.localScale.x<0){
                transform.localScale = new Vector3(m_scaleX,transform.localScale.y,transform.localScale.z);
            }
        }
        else{
            if(transform.position.x < enemy.gameObject.transform.position.x && transform.localScale.x<0){
                transform.localScale = new Vector3(-m_scaleX,transform.localScale.y,transform.localScale.z);
            }
            else if(transform.position.x > enemy.gameObject.transform.position.x && transform.localScale.x>0){
                transform.localScale = new Vector3(m_scaleX,transform.localScale.y,transform.localScale.z);
            }
        }
    }
    void Movement(){
        m_slashCDPass += Time.deltaTime;
        m_hardSlashCDPass += Time.deltaTime;
        m_counterCDPass += Time.deltaTime;

        m_moveDirection = Vector3.zero;
        //Jump
        if(Input.GetButtonDown(bJump) && isTouchingGround && !animator.GetBool("isHurt"))
        {
            isPressJump = true;
            m_jumpTime = 0;
        }
        if(isPressJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, m_jumpForce);
            m_jumpTime += Time.deltaTime;
            animator.SetBool("isJump", true);
        }
        if(Input.GetButtonUp(bJump) | m_jumpTime > m_jumpTimeMax){
            isPressJump = false;
        }

        //Attack
        if(Input.GetButtonDown(bSlash) && m_slashCDPass > m_slashCD)
        {
            m_slashCDPass = 0;
            SetContinuousAttack();
            animator.SetBool("isSlash", true);
        }
        if(Input.GetButtonDown(bHardSlash) && m_hardSlashCDPass > m_hardSlashCD)
        {
            m_hardSlashCDPass = 0;
            SetContinuousAttack();
            animator.SetBool("isHardSlash", true);
        }
        if(Input.GetButtonDown(bCounter) && m_counterCDPass > m_counterCD)
        {
            m_counterCDPass = 0;
            SetContinuousAttack();
            animator.SetBool("isCounter", true);
        }

        //Move
        if((Input.GetButton(bLeft) || Input.GetButton(bRight)) 
            && !animator.GetBool("isHurt") 
            && !animator.GetBool("isSlash") 
            && !animator.GetBool("isHardSlash")
            && !animator.GetBool("isCounter")
            && !animator.GetBool("isDead"))
        {
            animator.SetBool("isRun", true);
            if(Input.GetButton(bLeft))
            {
                m_moveDirection = Vector3.left;
            }
            if(Input.GetButton(bRight))
            {
                m_moveDirection = Vector3.right;
            }
        }
        else
        {
            animator.SetBool("isRun", false);
        }

        
        if(isTouchingGround){
            animator.SetBool("isJump", false);
        }
        transform.Translate(m_moveDirection * m_speed * Time.deltaTime);
    }
    
    void GroundCheck()
    {
        isTouchingGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
    void OnTriggerEnter2D(Collider2D other){
        if(slashCollider.activeSelf && other.gameObject.CompareTag("HardSlash")
           || hardSlashCollider.activeSelf && other.gameObject.CompareTag("Counter")
           || counterCollider.activeSelf && other.gameObject.CompareTag("Slash"))
        {
            soundSource.PlayOneShot(soundCounter);
            enemy.GetComponent<PlayerController>().TakeDamage(5);
            StartCoroutine(DisplayCounterText());
        }
        else if(other.gameObject.CompareTag("Player")){
            enemy.GetComponent<PlayerController>().TakeDamage(1);
        }
    }
    void AttackCollider()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Slash")){
            slashCollider.SetActive(true);
        }
        else if(animator.GetCurrentAnimatorStateInfo(0).IsName("HardSlash")){
            hardSlashCollider.SetActive(true);
        }
        else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Counter")){
            counterCollider.SetActive(true);
        }
    }
    void AnimationFinish(){
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Slash")){
            animator.SetBool("isSlash", false);
            slashCollider.SetActive(false);
        }
        else if(animator.GetCurrentAnimatorStateInfo(0).IsName("HardSlash")){
            animator.SetBool("isHardSlash", false);
            hardSlashCollider.SetActive(false);
        }
        else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Counter")){
            animator.SetBool("isCounter", false);   
            counterCollider.SetActive(false);
        }
        else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Hurt")){
            if(m_healhPoint<=0 && !animator.GetBool("isDead"))
            {
                animator.SetBool("isDead", true);
            }
            else
            {
                animator.SetBool("isHurt", false);
            }
        }
    }
    void AttackAnimationStart()
    {
        soundSource.PlayOneShot(soundAttack);
        if(animator.GetBool("isContinuousAttack"))
        {
            animator.SetBool("isContinuousAttack", false);
        }
    }
    void TakeDamage(int damage)
    {
        soundSource.PlayOneShot(soundDamage);
        animator.SetBool("isHurt", true);
        animator.SetBool("isSlash", false);
        animator.SetBool("isHardSlash", false);
        animator.SetBool("isCounter", false);
        slashCollider.SetActive(false);
        hardSlashCollider.SetActive(false);
        counterCollider.SetActive(false);
        m_healhPoint -= damage;
    }
    IEnumerator DisplayCounterText()
    {
        counterText.SetActive(true);
        counterText.transform.position = transform.position;
        yield return new WaitForSeconds(1);
        counterText.SetActive(false);
    }
    void SetContinuousAttack(){
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Slash") 
           || animator.GetCurrentAnimatorStateInfo(0).IsName("HardSlash")
           || animator.GetCurrentAnimatorStateInfo(0).IsName("Counter"))
        {
            animator.SetBool("isContinuousAttack", true);
        }
    }
}
