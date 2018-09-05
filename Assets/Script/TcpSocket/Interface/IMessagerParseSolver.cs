using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CALLBACK(byte[] data);

public interface IMessagerParseSolver{

    event CALLBACK MessageFixedUpCallback;

    void MessageSolver(object data);
}
