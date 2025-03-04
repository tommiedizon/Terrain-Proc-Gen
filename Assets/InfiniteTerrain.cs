﻿using UnityEngine;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine.UIElements;

public class InfiniteTerrain : MonoBehaviour {

    public static InfiniteTerrain Instance { get; private set; }

    // Pre calculated constants
    // 240 chosen due to a sufficient number of factors
    public const int CHUNK_SIZE = 240;
    public static readonly int[] CHUNK_SIZE_FACTORS = { 1, 2, 3, 4, 6, 8, 10, 12 };

    [SerializeField] Transform viewer;
    [SerializeField] float renderDistance = CHUNK_SIZE;

    int chunksRenderDistance;
    Vector2 viewerChunkCoords;

    private Dictionary<Vector2, TerrainChunk> terrainChunks = new();

    private void Awake() {
        if (Instance == null) Instance = this;
    }
    private void Start() {
        UpdateVariables();
    }

    private void Update() {

        UpdateVariables();
        UpdateChunks();

    }

    private void UpdateChunks() {
        for (int yOffset = -chunksRenderDistance; yOffset <= chunksRenderDistance; yOffset++) {
            for (int xOffset = -chunksRenderDistance; xOffset <= chunksRenderDistance; xOffset++) {
                var specificChunkCoords = new Vector2(viewerChunkCoords.x + xOffset, viewerChunkCoords.y + yOffset);

                int levelOfDetailIndex = 0;

                if (terrainChunks.ContainsKey(specificChunkCoords)) {
                    terrainChunks[specificChunkCoords].UpdateTerrainChunk();
                } else {
                    terrainChunks.Add(specificChunkCoords, new TerrainChunk(specificChunkCoords, levelOfDetailIndex));
                }
            }
        }
    }
    private void UpdateVariables() {
        viewerChunkCoords = new Vector2(Mathf.FloorToInt((float)viewer.position.x / CHUNK_SIZE), Mathf.FloorToInt((float)viewer.position.z / CHUNK_SIZE));
        chunksRenderDistance = Mathf.RoundToInt(renderDistance / CHUNK_SIZE);
    }

    // Getter
    public Vector2 GetViewerPosition() { return new Vector2(viewer.position.x, viewer.position.z); }
    public float GetRenderDistance() { return renderDistance; }

}

public class TerrainChunk {
    // CONSTS
    private const float DEFAULT_SCALE = 1.0f;

    // VARS
    private const int CHUNK_SIZE = InfiniteTerrain.CHUNK_SIZE;
    private readonly GameObject chunkObject;

    Bounds bounds; // Used for retrieving distance from chunk edge to viewer

    // CONSTRUCTOR
    public TerrainChunk(Vector2 viewerChunkCoords, int levelOfDetailIndex) {
        /* PARAM DESCRIPTIONS
         * viewerChunkCoords: The viewer's position in Chunk Coordinates (World coordinates / CHUNK_SIZE)
         * levelOfDetailIndex: The index of the factors of CHUNK_SIZE to use in generating the chunk
         */

        Mesh newMesh = PlaneMeshGenerator.GeneratePlaneMesh(CHUNK_SIZE, CHUNK_SIZE, DEFAULT_SCALE);

        chunkObject = new GameObject("Chunk", typeof(MeshFilter), typeof(MeshRenderer));
        chunkObject.GetComponent<MeshFilter>().mesh = newMesh;
        chunkObject.GetComponent<MeshRenderer>().material = UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline.defaultMaterial;
        chunkObject.transform.position = new Vector3(viewerChunkCoords.x, 0f, viewerChunkCoords.y) * CHUNK_SIZE;

        var viewerChunkCoordsCenter = new Vector2(viewerChunkCoords.x + 0.5f, viewerChunkCoords.y + 0.5f);
        bounds = new Bounds(viewerChunkCoordsCenter * CHUNK_SIZE, Vector2.one * (float)CHUNK_SIZE/2);
    }

    // METHODS
    public void UpdateTerrainChunk() {
        float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(InfiniteTerrain.Instance.GetViewerPosition()));
        bool isVisible = viewerDstFromNearestEdge <= InfiniteTerrain.Instance.GetRenderDistance();
        SetVisible(isVisible);
    }

    public void UpdateTerrainChunkLOD() {
        // TODO: Function for updating a chunk's LOD after viewer moves further from it
    }
    
    // SETTERS & GETTERS
    private void SetVisible(bool isVisible) {
        chunkObject.SetActive(isVisible);
    }
}
