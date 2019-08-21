using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrzeprawaPromem
{
    public class Ferry
    {
        public static int ferryCapacity = 3, waitingTime = 400;

        private int posX;
        private Window w;

        public Ferry(Window w)
        {
            posX = 600;
            this.w = w;
            Thread t1 = new Thread(ThisSideLimitReached);
            t1.IsBackground = true;
            t1.Start();
            Thread t2 = new Thread(OtherSideLimitReached);
            t2.IsBackground = true;
            t2.Start();
            Thread t3 = new Thread(TimeOut);
            t3.IsBackground = true;
            t3.Start();
            Thread t4 = new Thread(Run);
            t4.IsBackground = true;
            t4.Start();
        }

        public void ThisSideLimitReached()
        {
            while (true)
            {
                w.flag1.WaitOne();
                w.handlingOccurenceSem.WaitOne();
                w.handlingOccurenceSem.Release();
                if (w.passengers.Count == ferryCapacity)
                {
                    w.handlingOccurenceSem.WaitOne();
                    w.ferryOnLeftSem.WaitOne(0);
                    w.ferryOnRightSem.WaitOne(0);
                    w.ferryMovingSem.Release();
                }
                w.flag2.Release();
            }
        }

        public void OtherSideLimitReached()
        {
            while (true)
            {
                w.flag2.WaitOne();
                w.handlingOccurenceSem.WaitOne();
                w.handlingOccurenceSem.Release();
                if (w.ferryOnRightSem.WaitOne(0))
                {
                    w.ferryOnRightSem.Release();
                    if (w.leftQueue.Count >= ferryCapacity && w.rightQueue.Count == 0) 
                    {
                        w.handlingOccurenceSem.WaitOne();
                        w.ferryOnLeftSem.WaitOne(0);
                        w.ferryOnRightSem.WaitOne(0);
                        w.ferryMovingSem.Release();
                    }
                }
                if(w.ferryOnLeftSem.WaitOne(0))
                {
                    w.ferryOnLeftSem.Release();
                    if (w.rightQueue.Count >= ferryCapacity && w.leftQueue.Count == 0)
                    {
                        w.handlingOccurenceSem.WaitOne();
                        w.ferryOnLeftSem.WaitOne(0);
                        w.ferryOnRightSem.WaitOne(0);
                        w.ferryMovingSem.Release();
                    }
                }
                w.flag3.Release();
            }
        }

        public void TimeOut()
        {
            while (true)
            {
                w.flag3.WaitOne();
                w.handlingOccurenceSem.WaitOne();
                w.handlingOccurenceSem.Release();
                if (w.timeFerryStanding >= waitingTime)
                {
                    w.handlingOccurenceSem.WaitOne();
                    w.ferryOnLeftSem.WaitOne(0);
                    w.ferryOnRightSem.WaitOne(0);
                    w.ferryMovingSem.Release();
                }
                w.flag1.Release();
            }
        }

        public void Run()
        {
            while (true)
            {
                w.handlingOccurenceSem.Release();
                w.ferryMovingSem.WaitOne();
                w.gettingOnFerrySem.WaitOne();
                while (posX > 400)
                {
                    Move(-1);
                    foreach (Car c in w.passengers) c.Move(-1);
                    Thread.Sleep(10);
                }
                w.timeFerryStanding = 0;
                w.gettingOnFerrySem.Release();
                ReleasePassengers();
                w.ferryOnLeftSem.Release();
                for (int i = 0; i < ferryCapacity; i++) w.leftLoad.WaitOne(0);
                for (int i = 0; i < ferryCapacity; i++) w.leftLoad.Release();

                w.handlingOccurenceSem.Release();
                w.ferryMovingSem.WaitOne();
                w.gettingOnFerrySem.WaitOne();
                while (posX < 600)
                {
                    Move(1);
                    foreach(Car c in w.passengers) c.Move(1);
                    Thread.Sleep(10);
                }
                w.timeFerryStanding = 0;
                w.gettingOnFerrySem.Release();
                ReleasePassengers();
                w.ferryOnRightSem.Release();
                for (int i = 0; i < ferryCapacity; i++) w.rightLoad.WaitOne(0);
                for (int i = 0; i < ferryCapacity; i++) w.rightLoad.Release();
            }
        }

        private void ReleasePassengers()
        {
            for(int i=ferryCapacity; i>0; i--)
            {
                if(w.passengers.Count == i)
                {
                    for(int j=0; j<i; j++)
                    {
                        w.ferryCapacitySem.Release();
                    }
                }
            }
            w.passengers.RemoveRange(0, w.passengers.Count);
        }

        public void Move(int speed)
        {
            posX += speed;
        }

        public int getX()
        {
            return posX;
        }
    }
}
