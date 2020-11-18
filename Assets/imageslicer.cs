using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class imageslicer
{
    public static Texture2D[,] GetSlices(Texture2D image, int _gridSize)
    {
        int imageSize = Mathf.Min(image.width, image.height);
        int blockSize = imageSize / _gridSize;

        Texture2D[,] blocks = new Texture2D[_gridSize, _gridSize];

        for (int y = 0; y < _gridSize; y++)
        {
            for (int x = 0; x < _gridSize; x++)
            {
                Texture2D block = new Texture2D(blockSize, blockSize);
                block.wrapMode = TextureWrapMode.Clamp;
                block.SetPixels(image.GetPixels(x * blockSize, y * blockSize, blockSize, blockSize));
                block.Apply();
                blocks[x, y] = block;
            }
        }

        return blocks;
    }

}
