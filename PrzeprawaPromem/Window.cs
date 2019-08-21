using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrzeprawaPromem
{
    public partial class Window : Form
    {
        private Ferry f;

        public int timeFerryStanding;

        public List<Car> cars, leftQueue, rightQueue, passengers;

        public Semaphore    leftCarsMakingSem, rightCarsMakingSem, paintingSem,
                            flag1, flag2, flag3, handlingOccurenceSem,
                            leftLoad, rightLoad,
                            ferryCapacitySem, ferryOnLeftSem, ferryOnRightSem,
                            ferryMovingSem, gettingOnFerrySem, carGettingOffSem;
                                    

        public Window()
        {
            InitializeComponent();
            InitializeSemaphores();
            cars = new List<Car>();
            leftQueue = new List<Car>();
            rightQueue = new List<Car>();
            passengers = new List<Car>();
            f = new Ferry(this);
            Thread leftCarsMaker = new Thread(MakeLeftCars);
            leftCarsMaker.IsBackground = true;
            leftCarsMaker.Start();
            Thread rightCarsMaker = new Thread(MakeRightCars);
            rightCarsMaker.IsBackground = true;
            rightCarsMaker.Start();
        }
        
        private void InitializeSemaphores()
        {
            leftCarsMakingSem = new Semaphore(3, 5);
            rightCarsMakingSem = new Semaphore(3, 5);
            paintingSem = new Semaphore(1, 1);
            flag1 = new Semaphore(1, 1);
            flag2 = new Semaphore(0, 1);
            flag3 = new Semaphore(0, 1);
            ferryCapacitySem = new Semaphore(Ferry.ferryCapacity, Ferry.ferryCapacity);
            ferryOnLeftSem = new Semaphore(0, 1);
            ferryOnRightSem = new Semaphore(1, 1);
            ferryMovingSem = new Semaphore(0, 1);
            handlingOccurenceSem = new Semaphore(0, 1);
            gettingOnFerrySem = new Semaphore(1, 1);
            carGettingOffSem = new Semaphore(1, 1);
            leftLoad = new Semaphore(Ferry.ferryCapacity, Ferry.ferryCapacity);
            rightLoad = new Semaphore(Ferry.ferryCapacity, Ferry.ferryCapacity);
        }

        private void MakeLeftCars()
        {
            Random delayR = new Random();
            int delay;

            Thread.Sleep(delayR.Next(0, 2000));
            while (true)
            {
                leftCarsMakingSem.WaitOne();
                Car c = new Car(this,true);
                paintingSem.WaitOne();
                cars.Add(c);
                paintingSem.Release();
                leftQueue.Add(c);

                delay = delayR.Next(0, 4);
                if (delay == 0) Thread.Sleep(delayR.Next(0, 2000));
                else if (delay == 1) Thread.Sleep(delayR.Next(4000, 7000));
                else if (delay == 3) Thread.Sleep(delayR.Next(10000, 15000));
                Thread.Sleep(800);
            }
        }

        private void MakeRightCars()
        {
            Random delayR = new Random();
            int delay;
          
            while (true)
            {
                rightCarsMakingSem.WaitOne();
                Random r1 = new Random();
                Car c = new Car(this,false);
                paintingSem.WaitOne();
                cars.Add(c);
                paintingSem.Release();
                rightQueue.Add(c);

                delay = delayR.Next(0, 3);
                if (delay == 0) Thread.Sleep(delayR.Next(0, 2000));
                else if (delay == 1) Thread.Sleep(delayR.Next(4000, 7000));
                Thread.Sleep(800);
            }
        }

        private void Projekt_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Salmon, f.getX(), 200, 100, 200);
            paintingSem.WaitOne();
            foreach (Car c in cars)
            {
                e.Graphics.FillRectangle(Brushes.Brown, c.getX(), c.getY(), 60, 40);
                e.Graphics.DrawRectangle(Pens.NavajoWhite, c.getX(), c.getY(), 60, 40);
            }
            paintingSem.Release();
            e.Graphics.FillRectangle(Brushes.Black, f.getX() + 10, 210, 80, 180);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timeFerryStanding++;
            Invalidate();
        }

        private void Projekt_Load(object sender, EventArgs e)
        {

        }
    }
}
