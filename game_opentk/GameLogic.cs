using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Data;
using System.Drawing.Imaging;
using System.Collections;

namespace game_opentk
{
    class Ball
    {      
    // позиция мяча
    private float[] position = new float[3]; 
    // время жизни 
    private float _lifeTime; 
    // вектор гравитации 
    private float[] Grav = new float[3];  
    // набранная скорость 
    private float[] speed = new float[3]; 
    // временной интервал активации мяча
    private float LastTime = 0;     

    // конструктор класса 
    public Ball(float x, float y, float z, float lifeTime, float start_time) 
    { 
      // записываем все начальные настройки мяча, устанавливаем начальный коэффициент затухания 
      // и обнуляем скорость и силу приложенную к мячу 
      
      _lifeTime = lifeTime; 

      position[0] = x; 
      position[1] = y; 
      position[2] = z; 

      speed[0] = 0; 
      speed[1] = 0; 
      speed[2] = 0; 

      Grav[0] = 0; 
      Grav[2] = -9800f; 
      Grav[1] = 0; 
      
      LastTime = start_time;
    } 
    
    // инвертирование скорости мяча по заданной оси с указанным затуханием, при ударе о поверхности
    public void InvertSpeed( int os, float attenuation) 
    { 
      speed[os] *= -1 * attenuation; 
    } 

    // установка начальной скорости мяча
    public void setSpeed( float v_x, float v_y, float v_z)
    {
        speed[0] = v_x;
        speed[1] = v_y;
        speed[2] = v_z;
    }
    
    // обновление позиции мяча 
    public void UpdatePosition( float timeNow) 
    {
      // определяем разницу во времени, прошедшего с последнего обновления 
      // позиции мяча (ведь таймер может быть не фиксированный) 
      float dTime = (timeNow - LastTime); 
      _lifeTime -= dTime; 
        
      // обновляем последнюю отметку временного интервала 
      LastTime = timeNow;

      if (position[2] < -8.95f)
      {
          if (speed[0] > 0 )
              speed[0] -= (3f) * dTime;
          else
              speed[0] += (3f) * dTime;          
          if (speed[1] > 0)
              speed[1] -= (3f) * dTime;
          else
              speed[1] += (3f) * dTime;           
      }
           
      // перерасчитываем ускорение, движущее мяч 
      for ( int a = 0; a < 3; a++) 
      { 
        
        // перерасчитываем позицию мяча с учетом гравитации и прошедший промежуток времени 
        position[a] += (speed[a] * dTime + (Grav[a]) * dTime * dTime); 

        // обновляем скорость мяча
        speed[a] += (Grav[a]) * dTime;

      }

    } 

    // проверка, не вышло ли время жизни мяча
    public bool isLife() 
    { 
      if (_lifeTime > 0) 
      {
        return true ;
      } 
      else 
      {
        return false ;
      }
    } 

    // устоановить новые координаты мяча
    public void SetPosition( float _x, float _y, float _z)
    {
        position[0] = _x;
        position[1] = _y;
        position[2] = _z;
    }

    // получение координат мяча
    public float GetPositionX() 
    { 
      return position[0]; 
    } 
    public float GetPositionY() 
    { 
      return position[1]; 
    } 
    public float GetPositionZ() 
    { 
      return position[2]; 
    } 

  }
//====================================================================================
    class ThrowBall
    {
        // позиция мяча
        private float[] position = new float[3];       
        // активирован 
        private bool isStart = false;
        // экземпляр мяч на основе модели мяча
        private Ball ball;

        // конструктор класса, в него передаются координаты, где должен появиться мяч и начальная скорость броска
        public ThrowBall(float x, float y, float z)
        {
            position[0] = x;
            position[1] = y;
            position[2] = z;
            ball = new Ball(position[0], position[1], position[2], 10, 0);
        }

        // функция обновления позиции мяча 
        public void SetNewPosition(float x, float y, float z)
        {
            position[0] = x;
            position[1] = y;
            position[2] = z;
        }

        public void Throw(float time_start, float _speedX, float _speedY, float _speedZ)
        {
            // создаем мяч
            ball = new Ball(position[0], position[1], position[2], 10, time_start);
            ball.setSpeed(_speedX, _speedY, _speedZ);
           
            // бросок активирован 
            isStart = true;       

        }

        // калькуляция текущего броска
        public void Calculate(float time)
        {
            // только в том случае, если бросок уже активирован 
            if (isStart)
            {                
                 // если время жизни мяча еще не вышло 
                 if (ball.isLife())
                 {
                        // обновляем позицию мяча
                        ball.UpdatePosition(time);                       
                       
                        // Отражение от стены дальней
                        if (ball.GetPositionY()  > 39)
                        {
                            ball.SetPosition(ball.GetPositionX(), 39f, ball.GetPositionZ());
                            // инвертируем проекцию скорости на ось Y                          
                            ball.InvertSpeed(1, 0.8f);                            
                        }
                        // если координата Z стала меньше нуля (удар о землю) 
                        if (ball.GetPositionZ() < -9)
                        {
                            ball.SetPosition(ball.GetPositionX(), ball.GetPositionY(), -9);
                            ball.InvertSpeed(2, 0.8f);
                        }
                       //Отражение от левой стены
                        if (ball.GetPositionX() < -19)
                        {
                            ball.SetPosition(-19, ball.GetPositionY(), ball.GetPositionZ());
                            ball.InvertSpeed(0, 0.8f);
                        }
                       // Отражение от правой стены 
                        if (ball.GetPositionX() > 19)
                        {
                            ball.SetPosition(19, ball.GetPositionY(), ball.GetPositionZ());
                            ball.InvertSpeed(0, 0.8f);
                        }
                       //Отражение от потолка
                        if (ball.GetPositionZ() > 9)
                        {
                            ball.SetPosition(ball.GetPositionX(), ball.GetPositionY(), 9);
                            // инвертируем проекцию скорости на ось Y                          
                            ball.InvertSpeed(2, 0.8f);
                        }
                     
                     
                        
                    }
                }

            }

        // проверить жив ли мяч 
        public bool isLife()
        {
            if (isStart)
                return ball.isLife();
            return false;
        }

        // Получить позицию во время броска
        public float GetPositionX()
        {
            return position[0];
        }
        public float GetPositionY()
        {
            return position[1];
        }
        public float GetPositionZ()
        {
            return position[2];
        }
        // Получить позицию мяча
        public float GetBallPositionX()
        {
            if (isStart)
                return ball.GetPositionX();
            return 0;
        }
        public float GetBallPositionY()
        {
            if (isStart)
                return ball.GetPositionY();
            return 0;
        }
        public float GetBallPositionZ()
        {
            if (isStart)
                return ball.GetPositionZ();
            return 0;
        } 
        
     }
//====================================================================================   

    class GameQuad
    {
        private Color now;
        private Color next;
        private float life_time;
        private float last_time = 0;
        private float now_time = 0;
        private bool life = false;
        private float x, y, z, a;

        public GameQuad(Color _now, Color _next, float _life_time, float _x, float _y, float _z, float _a)
        {
            now = _now;
            next = _next;
            life_time = _life_time;
            x = _x;
            y = _y;
            z = _z;
            a = _a;
            life = true;
        }

        public void New(Color _next, float time)
        {
            last_time = 0;
            now_time = time;
            now = next;
            next = _next;
            life = true;
        }

        public bool Update(float time, Color mainC, Color color, float _x, float _y, float _z)
        {  
            if (life)
            {
                if (last_time < life_time)
                {
                    last_time += time - now_time;
                    now_time = time;
                }
                else
                {
                    this.New(color, time);
                }
            }
            else
                this.New(color, time);

            if (this.Hit(_x, _y, _z) && this.now == mainC)
                return true;

            return false;
        }

        public bool Hit(float _x, float _y, float _z)
        {
             if (y <= _y + 1 && (x - a < _x + 1) && (z - a / 2 < _z + 1) && (x + a > _x + 1) && (z + a / 2 > _z + 1))
            {
                life = false;
                return true;
            }
            return false;
        }

        public bool isLife()
        {
            return life;
        }
        public Color Now()
        {
            return now;
        }
        public float GetX()
        {
            return x;
        }
        public float GetY()
        {
            return y;
        }
        public float GetZ()
        {
            return z;
        }
        public float GetA()
        {
            return a;
        }


    }

} 











 