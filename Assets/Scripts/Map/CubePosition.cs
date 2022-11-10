using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePosition
{
    public int q, r, s;

    public CubePosition(int q, int r, int s)
    {
        this.q = q;
        this.r = r;
        this.s = s;
    }

    public CubePosition(Vector2 position)
    {
        OffsetToCube(position);
    }

    public CubePosition(float float_q, float float_r, float float_s)
    {
        int round_q = (int)Mathf.Round(float_q);
        int round_r = (int)Mathf.Round(float_r);
        int round_s = (int)Mathf.Round(float_s);

        float q_diff = Mathf.Abs(round_q - float_q);
        float r_diff = Mathf.Abs(round_r - float_r);
        float s_diff = Mathf.Abs(round_s - float_s);

        if (q_diff > r_diff && q_diff > s_diff)
            round_q = -round_r - round_s;
        else if (r_diff > s_diff)
            round_r = -round_q - round_s;
        else
            round_s = -round_q - round_r;

        q = round_q;
        r = round_r;
        s = round_s;
    }

    public Vector2 CubeToOffset()
    {
        Vector2 position;

        position.x = q + (r + (r % 2)) / 2;
        position.y = r;
        return position;
    }

    public void OffsetToCube(Vector2 position)
    {
        q = (int)position.x - ((int)position.y + ((int)position.y % 2)) / 2;
        r = (int)position.y;
        s = -q - r;
    }

    public int CheckDistance(CubePosition targetCell)
    {
        return Mathf.Max(Mathf.Abs(r - targetCell.r), Mathf.Abs(q - targetCell.q), Mathf.Abs(s - targetCell.s));
    } 
}
