using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OdorDiffusion : MonoBehaviour
{
    public float diffusionCoefficient = 0.1f; // difussion
    public float odorReleaseRate = 1.0f; // release rate
    public float odorDecayRate = 0.01f; // decay rate

    private float[,] _odorGrid; // gas grid
    private int _gridSizeX = 5; // grid size X
    private int _gridSizeZ = 5; // grid size Z

    //public GameObject cheesePlayer; // cheese player

    void Start()
    {
        // initialize the odor grid
        _odorGrid = new float[_gridSizeX, _gridSizeZ];

        // initialize the odor grid
        InitializeOdorGrid();

        // start the diffusion process
        InvokeRepeating("DiffuseOdor", 1f, 1f);
    }

    void InitializeOdorGrid()
    {
        // set the odor source
        Vector3 position = transform.position;
        int gridX = Mathf.RoundToInt(position.x);
        int gridZ = Mathf.RoundToInt(position.z);

        // make sure the position is within the valid range
        gridX = Mathf.Clamp(gridX, 0, _gridSizeX - 1);
        gridZ = Mathf.Clamp(gridZ, 0, _gridSizeZ - 1);

        _odorGrid[gridX, gridZ] = 1f; // set the odor source
    }

    void DiffuseOdor()
    {
        // iterate through each grid node
        for (int x = 1; x < _gridSizeX - 1; x++)
        {
            for (int z = 1; z < _gridSizeZ - 1; z++)
            {
                // calculate the change in odor concentration
                float deltaOdor = (_odorGrid[x - 1, z] + _odorGrid[x + 1, z] +
                                   _odorGrid[x, z - 1] + _odorGrid[x, z + 1]) * 0.25f - _odorGrid[x, z];

                // update the odor concentration
                _odorGrid[x, z] += odorReleaseRate * deltaOdor - odorDecayRate * _odorGrid[x, z];
            }
        }

        // update the odor particles
        UpdateOdorParticles();
    }

    void UpdateOdorParticles()
    {
        // get the particle system component
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();

        // get the main module of the particle system
        ParticleSystem.MainModule mainModule = particleSystem.main;

        // set the number of particles
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
        int particleCount = particleSystem.GetParticles(particles);

        // iterate through each particle
        for (int i = 0; i < particleCount; i++)
        {
            // get the position of the current particle
            Vector3 particlePosition = particles[i].position;

            // get the odor concentration at the particle position
            float odorConcentration = GetOdorConcentration(particlePosition);

            // adjust the size of the particle based on the odor concentration
            particles[i].startSize *= odorConcentration * diffusionCoefficient;

            // adjust the color of the particle based on the odor concentration
            particles[i].startColor = Color.Lerp(Color.white, Color.yellow, odorConcentration);
        }

        // update the particles
        particleSystem.SetParticles(particles, particleCount);
    }

    float GetOdorConcentration(Vector3 position)
    {
        // get the grid coordinates
        int x = Mathf.RoundToInt(position.x);
        int z = Mathf.RoundToInt(position.z);

        // make sure the coordinates are within the valid range
        x = Mathf.Clamp(x, 0, _gridSizeX - 1);
        z = Mathf.Clamp(z, 0, _gridSizeZ - 1);

        // return the odor concentration at the specified position
        return _odorGrid[x, z];
    }
}
