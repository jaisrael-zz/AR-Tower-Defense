#include <SFML/Window.hpp>
//Window.hpp automatically includes the OpenGL and GLU headers

//HOW TO OPEN A WINDOW IN SFML and use OPENGL with SFML
int main()
{

	//constructor for a Window Class
	//800x600 sized window with a depth of 32 bits per pixel
	//"SFML Window" is the window title
	sf::Window App(sf::VideoMode(800, 600, 32), "SFML Window");

//	App.Create(sf::VideoMode(800, 600, 32), "SFML Window", 
//			   sf::Style::Fullscreen);
  //App.Create(sf::VideoMode::GetMode(0), "SFML Window", sf::Style::Fullscreen);

	sf::Clock Clock;

	const float Speed = 50.f;
	float Left = 0.f;
	float Top = 0.f;
	
	//set color and depth clear value
	glClearDepth(1.f);
	glClearColor(0.f, 0.f, 0.f, 0.f);

	//enable z-buffer read and write
	glEnable(GL_DEPTH_TEST);
	glDepthMask(GL_TRUE);

	//Setup a perspective projection
	glMatrixMode(GL_PROJECTION);
	glLoadIdentity();
	//fov, aspect ratio, nearclip, farclip
	gluPerspective(90.f, 1.f, 1.f, 500.f);

	while (App.IsOpened())
	{
		App.Display();
		float time = Clock.GetElapsedTime();
		//Clock.Reset();
		sf::Event Event;
		while(App.GetEvent(Event))
		{
			if ((Event.Type == sf::Event::KeyPressed) &&
				(Event.Key.Code == sf::Key::Escape))
				App.Close();

			if (Event.Type == sf::Event::Closed)
				App.Close();
	
			if (Event.Type == sf::Event::Resized)
				glViewport(0, 0, Event.Size.Width, Event.Size.Height);
		}
	
		//Set the active window before using OpenGL commands
		//It's useless here because active window is always the same,
		//but don't forget it if you use multiple windows or controls
		App.SetActive();

		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		glMatrixMode(GL_MODELVIEW);
		glLoadIdentity();
		glTranslatef(0.f, 0.f, -200.f);
		glRotatef(Clock.GetElapsedTime()*50, 1.f, 0.f, 0.f);
		glRotatef(Clock.GetElapsedTime()*30, 0.f, 1.f, 0.f);
		glRotatef(Clock.GetElapsedTime()*90, 0.f, 0.f, 1.f);
	
		glBegin(GL_QUADS);

            glVertex3f(-50.f, -50.f, -50.f);
            glVertex3f(-50.f,  50.f, -50.f);
            glVertex3f( 50.f,  50.f, -50.f);
            glVertex3f( 50.f, -50.f, -50.f);

            glVertex3f(-50.f, -50.f, 50.f);
            glVertex3f(-50.f,  50.f, 50.f);
            glVertex3f( 50.f,  50.f, 50.f);
            glVertex3f( 50.f, -50.f, 50.f);

            glVertex3f(-50.f, -50.f, -50.f);
            glVertex3f(-50.f,  50.f, -50.f);
            glVertex3f(-50.f,  50.f,  50.f);
            glVertex3f(-50.f, -50.f,  50.f);

            glVertex3f(50.f, -50.f, -50.f);
            glVertex3f(50.f,  50.f, -50.f);
            glVertex3f(50.f,  50.f,  50.f);
            glVertex3f(50.f, -50.f,  50.f);

            glVertex3f(-50.f, -50.f,  50.f);
            glVertex3f(-50.f, -50.f, -50.f);
            glVertex3f( 50.f, -50.f, -50.f);
            glVertex3f( 50.f, -50.f,  50.f);

            glVertex3f(-50.f, 50.f,  50.f);
            glVertex3f(-50.f, 50.f, -50.f);
            glVertex3f( 50.f, 50.f, -50.f);
            glVertex3f( 50.f, 50.f,  50.f);

        glEnd();

		if (App.GetInput().IsKeyDown(sf::Key::Left))  Left -= Speed * time;
        if (App.GetInput().IsKeyDown(sf::Key::Right)) Left += Speed * time;
        if (App.GetInput().IsKeyDown(sf::Key::Up))    Top  -= Speed * time;
        if (App.GetInput().IsKeyDown(sf::Key::Down))  Top  += Speed * time;
//		App.Display();
	}
	return 0;
}
