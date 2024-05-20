using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunkSpawnController : MonoBehaviour
{
    GameObject[] chunks = new GameObject[4];
    TerrainChunk terrainScript;
    GameObject topLeftChunk, topRightChunk, bottomLeftChunk, bottomRightChunk;
    float halfTerrainSizeX = 0.0f, halfTerrainSizeY = 0.0f;
    bool OutsideTopLeftQuarter = false, OutsideTopRightQuarter = false, OutsideBottomLeftQuarter = false, OutsideBottomRightQuarter = false;
    bool OutsideTopEdge = false, OutsideBottomEdge = false, OutsideLeftEdge = false, OutsideRightEdge = false;
    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i< 4; i++)
        {
            chunks[i] = ObjectPoolManager.Instance.GetTerrainChunk();
        }
        terrainScript = chunks[0].GetComponent<TerrainChunk>();
        InitializeChunks();
    }
    void InitializeChunks()
    {
        chunks[0].transform.position = new Vector3(gameObject.transform.position.x, 0.0f, gameObject.transform.position.z);
        chunks[1].transform.position = new Vector3(gameObject.transform.position.x + terrainScript.Size.x, 0.0f, gameObject.transform.position.z);
        chunks[2].transform.position = new Vector3(gameObject.transform.position.x, 0.0f, gameObject.transform.position.z - terrainScript.Size.y);
        chunks[3].transform.position = new Vector3(gameObject.transform.position.x + terrainScript.Size.x, 0.0f, gameObject.transform.position.z - terrainScript.Size.y);
        topLeftChunk = chunks[0];
        topRightChunk = chunks[1];
        bottomLeftChunk = chunks[2];
        bottomRightChunk = chunks[3];
        halfTerrainSizeX = terrainScript.Size.x / 2.0f;
        halfTerrainSizeY = terrainScript.Size.y / 2.0f;
    }

    bool ShouldRearrangeChunks()
    {
        /*  ________
         *  | tl| tr| tl: top left, tr: top right
         *  |___|___|
         *  | bl| br| bl: bottom left, br: bottom right
         *  |___|___|
         *  
         *  we must re arrange chunks whenever the camera comes close to edges with a distance smaller than halfTerrainSizeX or halfTerrainSizeY
         *
         */
        OutsideTopLeftQuarter =
            ((topLeftChunk.transform.position.x - halfTerrainSizeX) > gameObject.transform.position.x) &&
            ((topLeftChunk.transform.position.z + halfTerrainSizeY) < gameObject.transform.position.z);

        OutsideTopRightQuarter = 
            ((topRightChunk.transform.position.x - halfTerrainSizeX) < gameObject.transform.position.x) &&
            ((topRightChunk.transform.position.z + halfTerrainSizeY) < gameObject.transform.position.z);

        OutsideBottomLeftQuarter =
            ((bottomLeftChunk.transform.position.x - halfTerrainSizeX) > gameObject.transform.position.x) &&
            ((bottomLeftChunk.transform.position.z + halfTerrainSizeY) > gameObject.transform.position.z);

        OutsideBottomRightQuarter =
            ((bottomRightChunk.transform.position.x - halfTerrainSizeX) < gameObject.transform.position.x) &&
            ((bottomRightChunk.transform.position.z + halfTerrainSizeY) > gameObject.transform.position.z);

        OutsideTopEdge = (topRightChunk.gameObject.transform.position.z + halfTerrainSizeY) < gameObject.transform.position.z;
        OutsideBottomEdge = (bottomLeftChunk.gameObject.transform.position.z + halfTerrainSizeY) > gameObject.transform.position.z;
        OutsideLeftEdge = (bottomLeftChunk.gameObject.transform.position.x - halfTerrainSizeX) > gameObject.transform.position.x;
        OutsideRightEdge = (topRightChunk.gameObject.transform.position.x - halfTerrainSizeY) < gameObject.transform.position.x;

        return OutsideTopLeftQuarter || OutsideTopRightQuarter || OutsideBottomLeftQuarter || OutsideBottomRightQuarter || 
            OutsideTopEdge || OutsideBottomEdge || OutsideLeftEdge || OutsideRightEdge;
    }
    void RearrengeChunks()
    {
        if (OutsideTopLeftQuarter)
        {
            /*  ________
             *  |*  |   |
             *  |___|___|
             *  |   |   |  * represents camera position
             *  |___|___|
             */
            MoveChunkOnX(ref topRightChunk, -2);
            MoveChunkOnY(ref bottomLeftChunk, +2);
            MoveChunkOnX(ref bottomRightChunk, -2);
            MoveChunkOnY(ref bottomRightChunk, +2);
            SwapChunks(ref topLeftChunk, ref bottomRightChunk);
            SwapChunks(ref topRightChunk, ref bottomLeftChunk);
        }
        else if (OutsideTopRightQuarter)
        {
            /*  ________
             *  |   |  *|
             *  |___|___|
             *  |   |   |  * represents camera position
             *  |___|___|
             */
            MoveChunkOnX(ref topLeftChunk, +2);
            MoveChunkOnY(ref bottomRightChunk, +2);
            MoveChunkOnX(ref bottomLeftChunk, +2);
            MoveChunkOnY(ref bottomLeftChunk, 2);
            SwapChunks(ref topRightChunk, ref bottomLeftChunk);
            SwapChunks(ref topLeftChunk, ref bottomRightChunk);
        }
        else if (OutsideBottomLeftQuarter)
        {
            /*  ________
             *  |   |   |
             *  |___|___|
             *  |   |   |  * represents camera position
             *  |*__|___|
             */
            MoveChunkOnX(ref bottomRightChunk, -2);
            MoveChunkOnY(ref topLeftChunk, -2);
            MoveChunkOnX(ref topRightChunk, -2);
            MoveChunkOnY(ref topRightChunk, -2);
            SwapChunks(ref topRightChunk, ref bottomLeftChunk);
            SwapChunks(ref topLeftChunk, ref bottomRightChunk);
        }
        else if (OutsideBottomRightQuarter)
        {
            /*  ________
             *  |   |   |
             *  |___|___|
             *  |   |   |  * represents camera position
             *  |___|__*|
             */
            MoveChunkOnX(ref bottomLeftChunk, +2);
            MoveChunkOnY(ref topRightChunk, -2);
            MoveChunkOnX(ref topLeftChunk, +2);
            MoveChunkOnY(ref topLeftChunk, -2);
            SwapChunks(ref topLeftChunk, ref bottomRightChunk);
            SwapChunks(ref topRightChunk, ref bottomLeftChunk);
        }
        else if (OutsideTopEdge)
        {
            /*  ________
             *  |   *   |
             *  |___|___|
             *  |   |   |  * represents camera position
             *  |___|___|
             */
            MoveChunkOnY(ref bottomLeftChunk, +2);
            MoveChunkOnY(ref bottomRightChunk, +2);
            SwapChunks(ref bottomLeftChunk, ref topLeftChunk);
            SwapChunks(ref bottomRightChunk, ref topRightChunk);
        }
        else if (OutsideLeftEdge)
        {
            /*  ________
             *  |   |   |
             *  |*__|___|
             *  |   |   |  * represents camera position
             *  |___|___|
             */
            MoveChunkOnX(ref topRightChunk, -2);
            MoveChunkOnX(ref bottomRightChunk, -2);
            SwapChunks(ref bottomLeftChunk, ref bottomRightChunk);
            SwapChunks(ref topLeftChunk, ref topRightChunk);
        }
        else if (OutsideRightEdge)
        {
            /*  ________
             *  |   |   |
             *  |___|__*|
             *  |   |   |  * represents camera position
             *  |___|___|
             */
            MoveChunkOnX(ref topLeftChunk, +2);
            MoveChunkOnX(ref bottomLeftChunk, +2);
            SwapChunks(ref bottomLeftChunk, ref bottomRightChunk);
            SwapChunks(ref topLeftChunk, ref topRightChunk);
        }
        else if (OutsideBottomEdge)
        {
            /*  ________
             *  |   |   |
             *  |___|___|
             *  |   |   |  * represents camera position
             *  |___*___|
             */
            MoveChunkOnY(ref topLeftChunk, -2);
            MoveChunkOnY(ref topRightChunk, -2);
            SwapChunks(ref topLeftChunk, ref bottomLeftChunk);
            SwapChunks(ref topRightChunk, ref bottomRightChunk);
        }
    }

    void MoveChunkOnX(ref GameObject chunk, int step)
    {
        chunk.transform.position += Vector3.right * step * halfTerrainSizeX * 2.0f;
    }

    void MoveChunkOnY(ref GameObject chunk, int step)
    {
        chunk.transform.position += Vector3.forward * step * halfTerrainSizeX * 2.0f;
    }

    void SwapChunks(ref GameObject lhsChunk, ref GameObject rhsChunk)
    {
        GameObject temp = lhsChunk;
        lhsChunk = rhsChunk;
        rhsChunk = temp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (ShouldRearrangeChunks())
        {
            RearrengeChunks();
        }
    }
}
