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

    //Dash
    public float m_dashSpeed;
    private bool isDashForward;

    //Ground check
    public Transform groundCheck;
    public float groundCheckRadius;
    [SerializeField] private bool isTouchingGround;
    public LayerMask groundLayer;
    //Detect attack
    public GameObject slashCollider;
    public GameObject hardSlashCollider;
    public GameObject counterCollider;
    public GameObject dashCollider;

    //Animator
    private Animator animator;

    //Button
    private string bLeft;
    private string bRight;
    private string bSlash;
    private string bShoot;
    private string bDash;
    private string bJump;
    private string bDown;

    //
    public PlayerController enemy;
    public int m_healhPoint;
    private bool isPlayer1;
    private float m_scaleX;
    public GameObject counterText;
    private bool isDamaged;
    
    //Shoot
    public GameObject Arrow;
    public GameObject ArrowSpawnPosition;

    //Skill cooldown
    public float m_slashCD;
    public float m_hardSlashCD;
    public float m_counterCD;
    private float m_slashCDPass;
    private float m_hardSlashCDPass;
    private float m_counterCDPass;
    private float m_dashCDPass;
    public float m_dashCD;
    private float m_shootCDPass;
    public float m_shootCD;

    //Audio
    public AudioClip soundAttack;
    public AudioClip soundCounter;
    public AudioClip soundDamage;
    public AudioClip soundShoot;
    public AudioClip soundDash;
    public AudioSource soundSource;

    // Start is called before the first frame update
    void Start()
    {
        m_scaleX = transform.localScale.x;
        if(gameObject.name == "Player1"){
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
            bShoot = "Shoot";
            bDash = "Dash";
            bJump = "Jump";
            bDown = "Down";
        }
        else if(gameObject.name == "Player2"){
            bLeft = "Move Left 2";
            bRight = "Move Right 2";
            bSlash = "Slash 2";
            bShoot = "Shoot 2";
            bDash = "Dash 2";
            bJump = "Jump 2";
            bDown = "Down 2";
        }
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        Movement();
        if(!animator.GetBool("isDash")){
            ChangeDirection();
        }
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
        m_dashCDPass += Time.deltaTime;
        m_shootCDPass += Time.deltaTime;

        m_moveDirection = Vector3.zero;
        //Jump
        if(Input.GetButtonDown(bJump) && isTouchingGround 
            && !animator.GetBool("isHurt") && !animator.GetBool("isDash")
            && !animator.GetBool("isSlash") && !animator.GetBool("isHardSlash") && !animator.GetBool("isCounter") && !animator.GetBool("isShoot"))
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
        //Slash
        if(!Input.GetButton(bDown) && Input.GetButtonDown(bSlash) && m_slashCDPass > m_slashCD)
        {
            m_slashCDPass = 0;
            SetContinuousAttack();
            animator.SetBool("isSlash", true);
        }
        //HardSlash
        if(!Input.GetButton(bDown) && Input.GetButtonDown(bShoot) && m_hardSlashCDPass > m_hardSlashCD)
        {
            m_hardSlashCDPass = 0;
            SetContinuousAttack();
            animator.SetBool("isHardSlash", true);
        }
        //Counter
        if(Input.GetButton(bDown) && Input.GetButtonDown(bSlash) && m_counterCDPass > m_counterCD)
        {
            m_counterCDPass = 0;
            SetContinuousAttack();
            animator.SetBool("isCounter", true);
        }
        //Shoot
        if(Input.GetButton(bDown) && Input.GetButtonDown(bShoot) && m_shootCDPass > m_shootCD)
        {
            m_shootCDPass = 0;
            animator.SetBool("isShoot", true);

        }
        //Dash
        if(!Input.GetButton(bDown) && Input.GetButtonDown(bDash) && m_dashCDPass > m_dashCD 
            && !animator.GetBool("isJump") && !animator.GetBool("isHurt"))
        {
            soundSource.PlayOneShot(soundDash);
            animator.SetBool("isDash", true);
            isDashForward = true;
        }
        if(Input.GetButton(bDown) && Input.GetButtonDown(bDash) && m_dashCDPass > m_dashCD 
            && !animator.GetBool("isJump") && !animator.GetBool("isHurt"))
        {
            soundSource.PlayOneShot(soundDash);
            animator.SetBool("isDash", true);
            isDashForward = false;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Dash"))
        {
            if(transform.localScale.x>0){
                transform.Translate(Vector3.right * m_dashSpeed * Time.deltaTime);
            }
            else{
                transform.Translate(Vector3.left * m_dashSpeed * Time.deltaTime);
            }
        }

        //Move
        if(((Input.GetButton(bLeft) || Input.GetButton(bRight)) && !animator.GetBool("isDash") && !animator.GetBool("isShoot")) 
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
        else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Dash")){
            dashCollider.SetActive(true);
        }
    }
    void OnTriggerEnter2D(Collider2D other){
        if((slashCollider.activeSelf && (other.gameObject.CompareTag("HardSlash") || other.gameObject.CompareTag("Dash")))
            || (hardSlashCollider.activeSelf && (other.gameObject.CompareTag("Counter") || other.gameObject.CompareTag("Dash"))) 
            || (counterCollider.activeSelf && (other.gameObject.CompareTag("Slash") || other.gameObject.CompareTag("Dash"))))
        {
            soundSource.PlayOneShot(soundCounter);
            enemy.GetComponent<PlayerController>().TakeDamage(5);
            StartCoroutine(DisplayCounterText());
        }
        else if(other.gameObject.CompareTag("Player")){
            enemy.GetComponent<PlayerController>().TakeDamage(1);
        }
        if((slashCollider.activeSelf || hardSlashCollider.activeSelf || counterCollider.activeSelf) && other.gameObject.CompareTag("Arrow")){
            soundSource.PlayOneShot(soundCounter);
            ObjectPoolManager.ReturnObjectToPool(other.gameObject);
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
        else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Dash")){
            animator.SetBool("isDash", false);   
            dashCollider.SetActive(false);
        }
        else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Shoot")){
            soundSource.PlayOneShot(soundShoot);
            animator.SetBool("isShoot", false);
            if(transform.localScale.x < 0){
                ArrowSpawnPosition.transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            else{
                ArrowSpawnPosition.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            ObjectPoolManager.SpawnObject(Arrow, ArrowSpawnPosition.transform.position, ArrowSpawnPosition.transform.rotation);
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
        else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Dash")){
            animator.SetBool("isDash", false);   
            // counterCollider.SetActive(false);
        }
    }
    void AttackAnimationStart()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Slash") || animator.GetCurrentAnimatorStateInfo(0).IsName("HardSash") || animator.GetCurrentAnimatorStateInfo(0).IsName("Counter")){
            soundSource.PlayOneShot(soundAttack);
        }
        if(animator.GetBool("isContinuousAttack"))
        {
            animator.SetBool("isContinuousAttack", false);
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Dash"))
        {
            m_dashCDPass = 0;
            if(!isDashForward){
                transform.localScale = new Vector3(-transform.localScale.x,transform.localScale.y,transform.localScale.z);
            }
        }
    }
    public void TakeDamage(int damage)
    {
        soundSource.PlayOneShot(soundDamage);
        animator.SetBool("isHurt", true);
        animator.SetBool("isSlash", false);
        animator.SetBool("isHardSlash", false);
        animator.SetBool("isCounter", false);
        animator.SetBool("isDash", false);
        slashCollider.SetActive(false);
        hardSlashCollider.SetActive(false);
        counterCollider.SetActive(false);
        dashCollider.SetActive(false);
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
