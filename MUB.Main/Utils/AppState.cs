﻿namespace MUB.Main.Utils;

public class AppState {
    public static bool IsDebug() {
#if DEBUG
        return true;
#else
        return false;
#endif
    }
}