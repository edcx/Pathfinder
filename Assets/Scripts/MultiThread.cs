﻿using System.Collections;
using System.Threading;

public class MultiThread {
    private bool m_IsDone   = false;
    private object m_Handle = new object();
    private Thread m_Thread = null;

    private int id = -1;
    public int Id { 
        get { return id; } 
        set { id = value; } 
    }

    public bool IsDone {
        get {
            bool tmp;
            lock (m_Handle) {
                tmp = m_IsDone;
            }
            return tmp;
        }
        set {

            lock (m_Handle)
            {
                m_IsDone = value;
            }
        }
    }

    public virtual void Start() {
        m_Thread = new Thread(Run);
        m_Thread.Start();
    }

    public virtual void Abort() {
        m_Thread.Abort();
    }

    protected virtual void ThreadFunction() { }

    protected virtual void OnFinished() { }

    private void Run() {
        ThreadFunction();
        IsDone = true;
    }


}
