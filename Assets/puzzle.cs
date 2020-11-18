using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class puzzle : MonoBehaviour
{


    [SerializeField]
    private Camera _puzzleCam;

    public Texture2D image;

    [SerializeField]
    private int _gridSize = 3;

    [SerializeField]
    private Slider _gridSizeSlider;

    [SerializeField]
    private float _moveDuration = .2f;

    [SerializeField]
    private int _shuffleLength = 20;

    [SerializeField]
    private float _shuffleMoveDuration = .1f;

    [SerializeField]
    private bool _numbersDisplayed = false;

    private GameObject canvas;


    enum PuzzleState { Solved, Shuffling, Inplay };


    [SerializeField]
    PuzzleState state;

    block emptyBlock;
    block[,] blockArray;
    Queue<block> inputs;
    bool isBlockMoving;
    int shuffleMovesRemaining;
    Vector2Int prevShuffleOffset;


    void Start()
    {
        _puzzleCam = Camera.main;
        canvas = GameObject.Find("Canvas");
        _gridSizeSlider = canvas.GetComponentInChildren<Slider>();
    }

    void Update()
    {
        _gridSize = Mathf.RoundToInt(_gridSizeSlider.value);


        if (state == PuzzleState.Solved && Input.GetKeyDown(KeyCode.Space))
        {
            StartShuffle();
        }

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            ScreenCapture.CaptureScreenshot("C:/screenshots/screenshot.png", 2);
        }
    }

    public void GeneratePuzzle()
    {
        _shuffleLength = _gridSize * _gridSize * _gridSize;
        _shuffleMoveDuration = _shuffleMoveDuration / _gridSize;


        blockArray = new block[_gridSize, _gridSize];

        Texture2D[,] imageSlices = imageslicer.GetSlices(image, _gridSize);

        _puzzleCam.orthographicSize = _gridSize * .55f;

        for (int y = 0; y < _gridSize; y++)
        {
            for (int x = 0; x < _gridSize; x++)
            {
                GameObject singleBlock = GameObject.CreatePrimitive(PrimitiveType.Quad);
                singleBlock.transform.position = -Vector2.one * (_gridSize - 1) * .5f + new Vector2(x, y);
                singleBlock.transform.parent = transform;

                block block = singleBlock.AddComponent<block>();
                block.OnBlockPressed += PlayerMoveBlockInput;
                block.OnFinishedMoving += OnBlockFinishedMoving;

                block.Init(new Vector2Int(x, y), imageSlices[x, y]);

                blockArray[x, y] = block;

                if (y == 0 && x == _gridSize - 1)
                {
                    emptyBlock = block;
                }
            }
        }

        inputs = new Queue<block>();
        StartShuffle();
    }

    private void PlayerMoveBlockInput(block blockToMove)
    {
        if (state == PuzzleState.Inplay)
        {
            inputs.Enqueue(blockToMove);
            MakeNextPlayerMove();
        }
    }

    void MakeNextPlayerMove()
    {
        while (inputs.Count > 0 && !isBlockMoving)
        {
            MoveBlock(inputs.Dequeue(), _moveDuration);
        }
    }

    private void MoveBlock(block blockToMove, float duration)
    {
        if ((blockToMove.coord - emptyBlock.coord).sqrMagnitude == 1)
        {
            blockArray[blockToMove.coord.x, blockToMove.coord.y] = emptyBlock;
            blockArray[emptyBlock.coord.x, emptyBlock.coord.y] = blockToMove;

            Vector2Int targetcoord = emptyBlock.coord;
            emptyBlock.coord = blockToMove.coord;
            blockToMove.coord = targetcoord;

            Vector2 targetPosition = emptyBlock.transform.position;
            emptyBlock.transform.position = blockToMove.transform.position;
            blockToMove.MoveToPosition(targetPosition, duration);
            isBlockMoving = true;
        }
    }

    void OnBlockFinishedMoving()
    {
        isBlockMoving = false;
        CheckIfSolved();

        if (state == PuzzleState.Inplay)
        {
            MakeNextPlayerMove();
        }
        else if (state == PuzzleState.Shuffling)
        {
            if (shuffleMovesRemaining > 0)
            {
                MakeNextShuffleMove();
            }
            else
            {
                state = PuzzleState.Inplay;
            }
        }


    }

    void StartShuffle()
    {
        state = PuzzleState.Shuffling;
        shuffleMovesRemaining = _shuffleLength;

        emptyBlock.gameObject.SetActive(false);
        
        MakeNextShuffleMove();
    }

    void MakeNextShuffleMove()
    {
        Vector2Int[] offsets = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };
        int randomIndex = Random.Range(0, offsets.Length);

        for (int i = 0; i < offsets.Length; i++)
        {
            Vector2Int offset = offsets[(randomIndex+i) % offsets.Length];


            if (offset != prevShuffleOffset *-1)
            {
                Vector2Int moveBlockCoord = emptyBlock.coord + offset;

                if (moveBlockCoord.x >= 0 && moveBlockCoord.x < _gridSize && moveBlockCoord.y >= 0 && moveBlockCoord.y < _gridSize)
                {
                    MoveBlock(blockArray[moveBlockCoord.x, moveBlockCoord.y], _shuffleMoveDuration);
                    shuffleMovesRemaining--;
                    prevShuffleOffset = offset;
                    break;
                }
            }
        }
    }

    void CheckIfSolved()
    {
        foreach (block block in blockArray)
        {
            if (!block.IsAtStartingCoord())
            {
                return;
            }
        }

        state = PuzzleState.Solved;
        emptyBlock.gameObject.SetActive(true);
    }

    public void DisplayNumbers()
    {
        if (_numbersDisplayed == false)
        {
            foreach (block block in blockArray)
            {

            }
        }
        else
        {

        }

    }


}
