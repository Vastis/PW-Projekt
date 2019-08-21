using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrzeprawaPromem
{
    public class Car
    {
        private int posX, posY;
        public bool leftSide;
        private Window w;

        public Car(Window w, bool leftSide)
        {
            this.w = w;
            this.leftSide = leftSide;
            Thread t = new Thread(CarHandler);
            t.IsBackground = true;
            t.Start();
        }
        
        private void CarHandler()
        {
            if (leftSide)
            {
                posX = -61;
                posY = 320;
                leftCar();
            }
            else
            {
                posX = 1171;
                posY = 240;
                rightCar();
            }
        }

        private void leftCar()
        {
            while (w.leftQueue.IndexOf(this) != 0)
            {
                while (posX <= 330 - 70 * w.leftQueue.IndexOf(this))
                {
                    Move(2);
                    Thread.Sleep(10);
                }
            }
            while (posX <= 330)
            {
                Move(2);
                Thread.Sleep(10);
            }

            w.ferryOnLeftSem.WaitOne();
            w.ferryOnLeftSem.Release();

            w.leftLoad.WaitOne();
            w.ferryCapacitySem.WaitOne();

            w.ferryOnLeftSem.WaitOne();
            w.ferryOnLeftSem.Release();
            w.gettingOnFerrySem.WaitOne();
            w.leftQueue.Remove(this);
            while (posX <= 420)
            {
                Move(2);
                Thread.Sleep(10);
            }
            w.passengers.Add(this);
            
            w.gettingOnFerrySem.Release();

            while (posX < 620) ;
            w.carGettingOffSem.WaitOne();
            while (posX < 700)
            {
                Move(2);
                Thread.Sleep(10);
            }
            w.carGettingOffSem.Release();
            while(posX < 1200)
            {
                Move(2);
                Thread.Sleep(10);
            }
            w.paintingSem.WaitOne();
            w.cars.Remove(this);
            w.leftCarsMakingSem.Release();
            w.paintingSem.Release();
        }

        private void rightCar()
        {
            while (w.rightQueue.IndexOf(this) != 0)
            {
                while (posX >= 720 + 70 * w.rightQueue.IndexOf(this))
                {
                    Move(-2);
                    Thread.Sleep(10);
                }
            }
            while (posX >= 720)
            {
                Move(-2);
                Thread.Sleep(10);
            }

            w.ferryOnRightSem.WaitOne();
            w.ferryOnRightSem.Release();

            w.rightLoad.WaitOne();
            w.ferryCapacitySem.WaitOne();

            w.ferryOnRightSem.WaitOne();
            w.ferryOnRightSem.Release();
            w.gettingOnFerrySem.WaitOne();
            w.rightQueue.Remove(this);
            while (posX >= 620)
            {
                Move(-2);
                Thread.Sleep(10);
            }
            w.passengers.Add(this);
            w.gettingOnFerrySem.Release();
            
            while (posX > 420) ;
            w.carGettingOffSem.WaitOne();
            while (posX > 340)
            {
                Move(-2);
                Thread.Sleep(10);
            }
            w.carGettingOffSem.Release();

            while (posX > -100)
            {
                Move(-2);
                Thread.Sleep(10);
            }

            w.paintingSem.WaitOne();
            w.cars.Remove(this);
            w.rightCarsMakingSem.Release();
            w.paintingSem.Release();
        }

        public void Move(int speed)
        {
            posX += speed;
        }

        public int getX()
        {
            return posX;
        }

        public int getY()
        {
            return posY;
        }
    }
}
