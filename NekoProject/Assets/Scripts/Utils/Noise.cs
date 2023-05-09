using System.Collections;
using UnityEngine;


/// <summary>
/// Regula el comportamiento del cubo con valores de ruido en su posición
/// </summary>
public class Noise : MonoBehaviour
{
    [SerializeField] bool floatOnX, floatOnY, floatOnZ;

    /// <summary>
    /// Máquina de estados que controla el scan del rudio
    /// </summary>
    enum IncrementState { ascending, descending }
    IncrementState incrementState;

    /// <summary>
    /// Posición inicial del objeto
    /// </summary>
    public Vector3 InitialPosition;

    //Variable auxiliar para el scan del ruido en el eje x de perlin para el eje X del objeto
    float xX;
    //Variable auxiliar para el scan del ruido en el eje x de perlin para el eje Y del objeto
    float xY;
    //Variable auxiliar para el scan del ruido en el eje x de perlin para el eje Z del objeto
    float xZ;

    //Variable auxiliar para el scan del ruido en el eje y
    float y;
    //Variable auxiliar para el máximo nivel de scan del ruido en el eje y
    float maxY = 1000;
    //Variable auxiliar para el mínimo nivel de scan del ruido en el eje y
    float minY = 10;

    //Variable auxiliar para el máximo nivel de scan del ruido en el eje y
    float maxX = 1000;
    //Variable auxiliar para el mínimo nivel de scan del ruido en el eje y
    float minX = 10;

    [Range(0, 5)]
    [SerializeField] float noiseMultiplier = 0.25f;
    [SerializeField] float noiseFrequency = 0.25f;

    Vector3 noiseIncrement;

    float beginMultiplier;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //Establece el estado inicial
        incrementState = IncrementState.ascending;

        //Guarda la posición inicial
        InitialPosition = transform.position;

        //Aletario entre dos valores
        xX = Random.Range(minX, maxX);
        xY = Random.Range(minX, maxX);
        xZ = Random.Range(minX, maxX);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        #region Incrementa o decrementa el ruido
        if (incrementState == IncrementState.ascending)
            y += Time.deltaTime * noiseFrequency;
        else
            y -= Time.deltaTime * noiseFrequency;
        #endregion

        #region Establece el ruido en la posición del objeto
        float _noiseX = Mathf.PerlinNoise(xX, y);  //Toma el ruido
        float _noiseY = Mathf.PerlinNoise(xY, y);  //Toma el ruido
        float _noiseZ = Mathf.PerlinNoise(xZ, y);  //Toma el ruido
                                                   //float ruido = Random.value;

        if (!floatOnX) _noiseX = 0;
        if (!floatOnY) _noiseY = 0;
        if (!floatOnZ) _noiseZ = 0;

        transform.position = new Vector3(
            InitialPosition.x + (_noiseX * noiseMultiplier * beginMultiplier),
            InitialPosition.y + (_noiseY * noiseMultiplier * beginMultiplier),
            InitialPosition.z + (_noiseZ * noiseMultiplier * beginMultiplier));  //Set de la posición
        #endregion

        #region Gestion del estado del incrmento
        if (y >= maxY)
            incrementState = IncrementState.descending;
        else if (y < minY)
            incrementState = IncrementState.ascending;
        #endregion
    }

    protected Vector3 CalculateNoise()
    {
        if (incrementState == IncrementState.ascending)
            y += Time.deltaTime * noiseFrequency;
        else
            y -= Time.deltaTime * noiseFrequency;

        float _noiseX = Mathf.PerlinNoise(xX, y);  //Toma el ruido
        float _noiseY = Mathf.PerlinNoise(xY, y);  //Toma el ruido
        float _noiseZ = Mathf.PerlinNoise(xZ, y);  //Toma el ruido
                                                   //float ruido = Random.value;
        if (!floatOnX) _noiseX = 0;
        if (!floatOnY) _noiseY = 0;
        if (!floatOnZ) _noiseZ = 0;

        return new Vector3(_noiseX * noiseMultiplier, _noiseY * noiseMultiplier, _noiseZ * noiseMultiplier);
    }

    protected void IncrementManagement()
    {
        #region Gestion del estado del incrmento
        if (y >= maxY)
            incrementState = IncrementState.descending;
        else if (y < minY)
            incrementState = IncrementState.ascending;
        #endregion
    }

    public IEnumerator Move(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) < .5f)
        {

            yield return null;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(Co_Begin());
    }

    IEnumerator Co_Begin()
    {
        beginMultiplier = 0;

        while(beginMultiplier < 1)
        {
            beginMultiplier += Time.deltaTime;
            yield return null;
        }

        beginMultiplier = 1;
    }
}
