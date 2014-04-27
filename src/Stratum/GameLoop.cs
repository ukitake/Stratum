using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using SharpDX.Toolkit;

namespace Stratum
{
    public class GameLoop
    {
        private Thread engineThread;

        public GameLoop()
        {
            IsFixedTimeStep = false;
            TargetFrameRate = 60;
            gameTimer = new Stopwatch();
        }

        public Thread EngineThread
        {
            get { return engineThread; }
        }

        private bool started;
        public void Start()
        {
            if (started)
                return;

            started = true;
            engineThread = new Thread(new ThreadStart(gameLoop));
            engineThread.Priority = ThreadPriority.Normal;
            engineThread.Start();
        }

        public bool ShouldExit { get; set; }

        public bool IsFixedTimeStep { get; set; }

        private int targetFrameRate;
        public int TargetFrameRate 
        {
            get { return targetFrameRate; }
            set 
            { 
                targetFrameRate = value; 
                this.targetElapsedTime = TimeSpan.FromTicks((long)10000000 / (long)targetFrameRate); 
            }
        }

        private Stopwatch gameTimer;
        private TimeSpan totalGameTime = TimeSpan.Zero;
        private TimeSpan elapsedGameTime = TimeSpan.Zero;
        private TimeSpan targetElapsedTime = TimeSpan.FromTicks((long)10000000 / (long)60);
        private readonly TimeSpan maxElapsedTime = TimeSpan.FromMilliseconds(500);
        private TimeSpan inactiveSleepTime = TimeSpan.FromSeconds(1);
        private TimeSpan accumulatedElapsedTime = TimeSpan.Zero;

        private DateTime _start;
        private DateTime _now;
        private DateTime _lastUpdate = DateTime.UtcNow;
        private GameTime _gameTime = new GameTime();
        private TimeSpan _fixedTimeStepTime = new TimeSpan();
        private TimeSpan _totalTime = TimeSpan.Zero;

        private void gameLoop()
        {
            _start = DateTime.UtcNow;
            while (!ShouldExit)
            {
                accumulatedElapsedTime += gameTimer.Elapsed;
                gameTimer.Reset();
                gameTimer.Start();

                if (IsFixedTimeStep && accumulatedElapsedTime < this.targetElapsedTime)
                {
                    var sleepTime = (int)(this.targetElapsedTime - accumulatedElapsedTime).TotalMilliseconds;
                    Thread.Sleep(sleepTime);
                    continue;
                }

                if (accumulatedElapsedTime > maxElapsedTime)
                    accumulatedElapsedTime = maxElapsedTime;

                if (IsFixedTimeStep)
                {
                    elapsedGameTime = this.targetElapsedTime;
                    var stepCount = 0;

                    bool isRunningSlowly = accumulatedElapsedTime > this.targetElapsedTime;

                    while (accumulatedElapsedTime >= this.targetElapsedTime)
                    {
                        totalGameTime += this.targetElapsedTime;
                        accumulatedElapsedTime -= this.targetElapsedTime;
                        stepCount++;

                        GameTime gt = new GameTime(totalGameTime, elapsedGameTime, isRunningSlowly);
                        Engine.Instance.Update(gt);
                    }

                    elapsedGameTime = TimeSpan.FromTicks(this.targetElapsedTime.Ticks * stepCount);
                }
                else
                {
                    elapsedGameTime = accumulatedElapsedTime;
                    totalGameTime += accumulatedElapsedTime;
                    accumulatedElapsedTime = TimeSpan.Zero;

                    GameTime gt = new GameTime(totalGameTime, elapsedGameTime, false);
                    Engine.Instance.Update(gt);
                }

                GameTime renderGameTime = new GameTime(totalGameTime, elapsedGameTime);
                Engine.Instance.Render(renderGameTime);
            }
        }
    }
}
