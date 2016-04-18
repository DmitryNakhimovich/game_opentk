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
    class BallModel
    {      
    // позиция мяча
    private float[] position = new float[3]; 
    // размер 
    private float _size; 
    // время жизни 
    private float _lifeTime; 
    // вектор гравитации 
    private float[] Grav = new float[3];  
    // набранная скорость 
    private float[] speed = new float[3]; 
    // временной интервал активации мяча
    private float LastTime = 0; 
    // текстурный объект 
    public uint TObj = 0;

    // конструктор класса 
    public BallModel(float x, float y, float z, float size, float lifeTime, float start_time) 
    { 
      // записываем все начальные настройки мяча, устанавливаем начальный коэффициент затухания 
      // и обнуляем скорость и силу приложенную к мячу 
      _size = size; 
      _lifeTime = lifeTime; 

      position[0] = x; 
      position[1] = y; 
      position[2] = z; 

      speed[0] = 0; 
      speed[1] = 0; 
      speed[2] = 0; 

      Grav[0] = 0; 
      Grav[1] = -9.8f; 
      Grav[2] = 0; 
      
      LastTime = start_time;
    } 
    
    // инвертирование скорости мяча по заданной оси с указанным затуханием, при ударе о поверхности
    public void InvertSpeed( int os, float attenuation) 
    { 
      speed[os] *= -1 * attenuation; 
    } 

    // получение размера мяча 
    public float GetSize() 
    { 
      return _size; 
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
      float dTime = timeNow - LastTime; 
      _lifeTime -= dTime; 

      // обновляем последнюю отметку временного интервала 
      LastTime = timeNow;

      if (position[1] < 0.01f)
      {
          if (speed[0] > 0 )
              speed[0] -= (3f) * dTime;
          else
              speed[0] += (3f) * dTime;
          if (speed[1] > 0)
              speed[1] -= (1.5f) * dTime;
          else
              speed[1] += (1.5f) * dTime;
          if (speed[2] > 0)
              speed[2] -= (3f) * dTime;
          else
              speed[2] += (3f) * dTime;           
      }

      // перерасчитываем ускорение, движущее мяч 
      for( int a = 0; a < 3; a++) 
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

    class ThrowBall
    {
        // позиция мяча
        private float[] position = new float[3];       
        // активирован 
        private bool isStart = false;
        // дисплейный список для рисования мяча создан 
        private bool isDisplayList = false;
        // номер дисплейного списка для отрисовки 
        private int DisplayListNom = 0;
        // экземпляр мяч на основе модели мяча
        private BallModel Ball;
        // размер мяча
        private float _size = 40;

        // конструктор класса, в него передаются координаты, где должен появиться мяч и начальная скорость броска
        public ThrowBall(float x, float y, float z)
        {
            position[0] = x;
            position[1] = y;
            position[2] = z;
            Ball = new BallModel(position[0], position[1], position[2], _size, 10, 0);
        }

        // функция обновления позиции мяча 
        public void SetNewPosition(float x, float y, float z)
        {
            position[0] = x;
            position[1] = y;
            position[2] = z;
        }

        // создания дисплейного списка для отрисовки мяча (т.к. отрисовывать даже небольшой полигон такое количество раз - очень накладно) 
        //private void CreateDisplayList()
        //{
        //    // генерация дисплейного списка 
        //    DisplayListNom = Gl.glGenLists(1);

        //    // начало создания списка 
        //    Gl.glNewList(DisplayListNom, Gl.GL_COMPILE);

        //    // режим отрисовки             
        //    Glu.GLUquadric quadr = Glu.gluNewQuadric();
        //    //задаем форму мяча                 
        //    Glu.gluQuadricTexture(quadr, Gl.GL_TRUE);
        //    Glu.gluSphere(quadr, 0.05, 150, 150);
        //    // завершаем отрисовку 
        //    Gl.glEndList();

        //    // флаг - дисплейный список создан 
        //    isDisplayList = true;
        //}

        // функция, реализующая бросок 
        public void Throw(float time_start, float _speedX, float _speedY, float _speedZ)
        {
            // если дисплейный список не создан - надо его создать 
            //if (!isDisplayList)
            //{
            //    CreateDisplayList();
            //}
            
            // создаем мяч
            Ball = new BallModel(position[0], position[1], position[2], _size, 10, time_start);
            Ball.setSpeed(_speedX, _speedY, _speedZ);
           
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
                 if (Ball.isLife())
                 {
                        // обновляем позицию мяча
                        Ball.UpdatePosition(time);                       
                        // сохраняем текущую матрицу 
                       // Gl.glPushMatrix();
                        // получаем размер мяча 
                        float size = Ball.GetSize();
                        // выполняем перемещение мяча в необходимую позицию 
                       // Gl.glTranslated(Ball.GetPositionX(), Ball.GetPositionY(), Ball.GetPositionZ());
                        // масштабируем ее в соответствии с ее размером 
                       // Gl.glScalef(size, size, size);
                        // вызываем дисплейный список для отрисовки частицы из кэша видеоадаптера 
                       // Gl.glCallList(DisplayListNom);
                        // возвращаем матрицу 
                       // Gl.glPopMatrix();      
 
                        // если координата Y стала меньше нуля (удар о землю) 
                        if (Ball.GetPositionY()  < -3f)
                        {
                            Ball.SetPosition(Ball.GetPositionX(), -3f, Ball.GetPositionZ());
                            // инвертируем проекцию скорости на ось Y                          
                            Ball.InvertSpeed(1, 0.8f);                            
                        }
                        // Отражение от стены дальней
                        if (Ball.GetPositionZ() < -25)
                        {
                            Ball.SetPosition(Ball.GetPositionX(), Ball.GetPositionY(), -25);
                            Ball.InvertSpeed(2, 0.8f);
                        }
                       //Отражение от левой стены
                        if (Ball.GetPositionX() < -5)
                        {
                            Ball.SetPosition(-5, Ball.GetPositionY(), Ball.GetPositionZ());
                            Ball.InvertSpeed(0, 0.8f);
                        }
                       // Отражение от правой стены 
                        if (Ball.GetPositionX() > 35)
                        {
                            Ball.SetPosition(35, Ball.GetPositionY(), Ball.GetPositionZ());
                            Ball.InvertSpeed(0, 0.8f);
                        }
                       //Отражение от потолка
                        if (Ball.GetPositionY() > 25)
                        {
                            Ball.SetPosition(Ball.GetPositionX(), 25, Ball.GetPositionZ());
                            // инвертируем проекцию скорости на ось Y                          
                            Ball.InvertSpeed(1, 0.8f);
                        }
                     // удар о кольцо 
                     
                        
                    }
                }

            }

        // проверить жив ли мяч 
        public bool isLife()
        {
            if (isStart)
                return Ball.isLife();
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
                return Ball.GetPositionX();
            return 0;
        }
        public float GetBallPositionY()
        {
            if (isStart)
                return Ball.GetPositionY();
            return 0;
        }
        public float GetBallPositionZ()
        {
            if (isStart)
                return Ball.GetPositionZ();
            return 0;
        } 
        // Получить размер мяча
        public float GetSize()
        {
            return Ball.GetSize();
        } 

     }
} 











 