using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static float GROUND_WIDTH = 432f;
    public static float GROUND_HALF_WIDTH = GROUND_WIDTH / 2;
    public static float PLAYER_LENGTH = 64f;
    public static float PLAYER_HALF_LENGTH = PLAYER_LENGTH / 2;
    public static float PLAYER_TOUCHING_GROUND_Y_COORD = 244;
    public static float BALL_RADIUS = 20;
    public static float BALL_TOUCHING_GROUND_Y_COORD = 252;
    public static float NET_PILLAR_HALF_WIDTH = 25;
    public static float NET_PILLAR_TOP_TOP_Y_COORD = 176;
    public static float NET_PILLAR_TOP_BOTTOM_Y_COORD = 192;

    public static float RELATIVE_GROUND = -6f;
    public static float INFINITE_LOOP_LIMIT = 2000;
}
