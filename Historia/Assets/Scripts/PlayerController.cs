using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _playerRigidbody2D;
    private Animator _playerAnimator;

    // Váriaveis de configuração de velocidade
    [Header("Configurações de Velocidade")]
    public float _playerRunSpeed = 8f;        // Velocidade de Corrida
    public float _playerNormalSpeed = 5f;     // Velocidade Padrão
    public float _playerSlowSpeed = 2f;       // Velocidade Lenta (Stealth)
    private float _currentSpeed;              // Velocidade atual a ser usada
    private Vector2 _rawInput;
    
    // Variáveis para persistência da direção
    private float _lastMoveX;
    private float _lastMoveY;

    // Configuração de teclas (Mantidas para a lógica de velocidade)
    [Header("Configuração de Teclas")]
    [Tooltip("Tecla ou botão para andar mais devagar (modo Stealth)")]
    public KeyCode _slowMoveKey = KeyCode.LeftControl; 
    [Tooltip("Tecla ou botão para Correr (Run)")]
    public KeyCode _runKey = KeyCode.LeftShift;

    void Start()
    {
        _playerRigidbody2D = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();

        // Define a velocidade inicial como a normal.
        _currentSpeed = _playerNormalSpeed; 

        if (_playerRigidbody2D == null)
        {
            Debug.LogError("O Rigidbody2D é necessário para o movimento!");
        }
    }

    void Update()
    {
        // 1. Lógica de Entrada (Input)
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Normaliza o vetor nas diagonais
        _rawInput = new Vector2(horizontalInput, verticalInput).normalized; 

        // 2. Lógica de Controle de Velocidade
        // Controla qual velocidade (_currentSpeed) será usada no FixedUpdate
        ControlPlayerSpeed();
    }

    void FixedUpdate()
    {
        Vector2 movementVector = _rawInput;

        // 3. Lógica de Movimento Físico
        if (movementVector.sqrMagnitude > 0.01f)
        {
            // Onde a velocidade é aplicada:
            MovePlayer(movementVector);
        }
        
        // A lógica do Animator deve idealmente ficar no LateUpdate ou Update.
        // Se ela ficasse aqui (FixedUpdate), ela seria:
        // MovePlayer(movementVector); // Movimento
        // _playerAnimator.SetFloat("AxisX", movementVector.x); // Animação (Blend Tree)
        // ...
    }
    
    void LateUpdate()
    {
        Vector2 movementVector = _rawInput;

        // 4. Lógica do Blend Tree (Animação)
        if (movementVector.sqrMagnitude > 0.01f)
        {
            // Seta os parâmetros do Blend Tree para a animação de ANDAR/CORRER
            _playerAnimator.SetFloat("AxisX", movementVector.x);
            _playerAnimator.SetFloat("AxisY", movementVector.y);
            
            // Armazena a última direção válida
            _lastMoveX = movementVector.x;
            _lastMoveY = movementVector.y;
            
            _playerAnimator.SetInteger("Movimento", 1); // Estado: Movimento
        }
        else
        {
            // Se NÃO houver movimento:
            
            _playerAnimator.SetInteger("Movimento", 0); // Estado: Idle (Parado) 
            
            // Usa a última direção armazenada para a Blend Tree de Idle
            _playerAnimator.SetFloat("LastMoveX", _lastMoveX);
            _playerAnimator.SetFloat("LastMoveY", _lastMoveY);
            
            _playerAnimator.SetFloat("AxisX", 0);
            _playerAnimator.SetFloat("AxisY", 0);
        }
    }
    

    // Onde a velocidade é controlada (Chama no Update)
    void ControlPlayerSpeed()
    {
        if (Input.GetKey(_slowMoveKey))
        {
            _currentSpeed = _playerSlowSpeed;
        }
        else if (Input.GetKey(_runKey))
        {
            _currentSpeed = _playerRunSpeed;
        }
        else
        {
            _currentSpeed = _playerNormalSpeed;
        }
        
        // DICA: Você pode querer um parâmetro no Animator como "SpeedMultiplier"
        // para dar feedback visual da velocidade (ex: animação de correr mais rápida)
        // _playerAnimator.SetFloat("SpeedMultiplier", _currentSpeed / _playerNormalSpeed); 
    }

    // Onde a velocidade é aplicada ao Rigidbody2D (Chama no FixedUpdate)
    void MovePlayer(Vector2 direction)
    {
        _playerRigidbody2D.MovePosition(_playerRigidbody2D.position + direction * _currentSpeed * Time.fixedDeltaTime);
    }
}