using System;
using System.Collections.Generic;

class TaskPack
{
    public int tid;
    public Action<int> cb;

    public TaskPack(int tid, Action<int> cb)
    {
        this.tid = tid;
        this.cb = cb;
    }
}

public class TimerSvc
{
    private static TimerSvc instance;

    public static TimerSvc Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TimerSvc();
            }

            return instance;
        }
    }

    private Timer timer;
    private static readonly string tpQueLock = "tpQueLock";
    Queue<TaskPack> tpQue = new Queue<TaskPack>();

    public void init()
    {
        timer = new Timer(100);
        tpQue.Clear();
        timer.SetLog(info => { CommonTool.Log(info); });
        timer.SetHandle((cb, tid) =>
        {
            if (cb != null)
            {
                lock (tpQueLock)
                {
                    tpQue.Enqueue(new TaskPack(tid, cb));
                }
            }
        });
        CommonTool.Log("TimerSvc Connected");
    }

    public void Update()
    {
        while (tpQue.Count > 0)
        {
            TaskPack tp = null;
            lock (tpQueLock)
            {
                tp = tpQue.Dequeue();
            }

            if (tp != null)
            {
                tp.cb(tp.tid);
            }
        }
    }

    public int AddTimeTask(Action<int> callback, double delay, TimeUnit timeunit = TimeUnit.Millisecond,
        int count = 1)
    {
        return timer.AddTimeTask(callback, delay, timeunit, count);
    }

    public long GetNowTime()
    {
        return (long)timer.GetMillisecondsTime();
    }
}