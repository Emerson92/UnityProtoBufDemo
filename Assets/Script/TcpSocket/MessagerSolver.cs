using cs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;

namespace MarsDT.TcpSocket
{

    public class MessagerSolver : IMessagerParseSolver
    {

        public MessagerSolver()
        {

        }

        public event CALLBACK MessageFixedUpCallback;

        public void MessageSolver(object data)
        {
            try
            {
                if (MessageFixedUpCallback != null)
                    MessageFixedUpCallback((byte[])data);
            }
            catch (Exception e) {
                Debug.Log(e.Message);
            }

        }
    }
}
