#include <opencv2/highgui/highgui.hpp>
#include <opencv2/core/core.hpp>
#include <opencv2/calib3d/calib3d.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/features2d/features2d.hpp>
#include <opencv2/nonfree/features2d.hpp>
#include <opencv2/opencv.hpp>
#include <SFML/Graphics.hpp>
#include <iostream>
#include <algorithm>
#include "math.h"

using namespace cv;
using namespace std;

int clicks;
int texture_created;
Mat image;
vector<Point2f> im0_corners(4); 
GLuint backgroundTextureID;


//click 4 points of the plane callback
static void onMouse(int event, int x, int y, int, void*)
{
	if (event != CV_EVENT_LBUTTONDOWN || clicks >= 4)
		return;
	Scalar color;
	switch (clicks)
	{
		case 0:
			color = Scalar(0,0,255);
			break;
		case 1:
			color = Scalar(0,255,0);
			break;
		case 2:
			color = Scalar(255,0,0);
			break;
		default:
			color = Scalar(0,255,255);
	}
	Point center = Point(x, y);
	im0_corners[clicks] = center; 
	circle(image, center, 4, color, -1); 
    cout << "(" << center.x << "," << center.y << ")" << endl;
	clicks++;
	
}
//default do nothing mouse callback
static void onMouse2(int event, int x, int y, int, void*)
{
	return;
}

//transformation with perspective divide
Point transform_corner(Mat H, Mat mp)
{
    Mat mp_new = H*mp;
	Point p = Point(mp_new.at<double>(0) / mp_new.at<double>(2),
					mp_new.at<double>(1) / mp_new.at<double>(2));
	return p;
}


void drawPlane(Mat im, Point p0, Point p1, Point p2, Point p3)
{ 
	circle(im, p0, 4, Scalar(0,0,255), -1); 
	circle(im, p1, 4, Scalar(0,255,0), -1); 
	circle(im, p2, 4, Scalar(255,0,0), -1); 
	circle(im, p3, 4, Scalar(0,255,255), -1); 

}

//computes the homography from image i to image i+1. 
Mat find_next_homography(Mat im, Mat image_next, vector<KeyPoint> keypoints_0, Mat descriptors_0,
						 Ptr<FeatureDetector> detector, Ptr<DescriptorExtractor> extractor, 
						 Ptr<DescriptorMatcher> matcher, vector<KeyPoint>& keypoints_next, 
						 Mat& descriptors_next)
{

	//step 1 detect feature points in next image
	vector<KeyPoint> keypoints_1;
	detector->detect(image_next, keypoints_1);

	Mat img_keypoints_surf0, img_keypoints_surf1;

    //step 2: extract feature descriptors from feature points
	Mat descriptors_1;
	extractor->compute(image_next, keypoints_1, descriptors_1);

	//step 3: feature matching
	//cout << "fd matching" << endl;
	vector<DMatch> matches;
	vector<Point2f> matched_0;
	vector<Point2f> matched_1;

	
	matcher->match(descriptors_0, descriptors_1, matches);
	//matcher->match(descriptors_0, matches);
	Mat img_feature_matches;
	//drawMatches(im, keypoints_0, image_next, keypoints_1, matches, img_feature_matches );
	//imshow("Matches", img_feature_matches);

	for (int i = 0; i < matches.size(); i++ )
	{
		matched_0.push_back(keypoints_0[matches[i].queryIdx].pt);	
		matched_1.push_back(keypoints_1[matches[i].trainIdx].pt);	
	}
	keypoints_next = keypoints_1;
	descriptors_next = descriptors_1;
	if (matches.size() < 4)
		return Mat::eye(3, 3, CV_64F);
	return findHomography(matched_0, matched_1, RANSAC);

}

void updateSFML(float elapsedTime, Mat backgroundImage, Mat K, Mat RT, double s)
{
	int w = image.cols;
	int h = image.rows;
	//Initialize texture
	if(!texture_created)
	{
		glGenTextures(1, &backgroundTextureID);
		glBindTexture(GL_TEXTURE_2D, backgroundTextureID);

  		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

		texture_created = 1;
	}

    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
	
	//draw background image (webcam frame)
	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, w, h, 0, GL_BGR_EXT, GL_UNSIGNED_BYTE, backgroundImage.data);

 
    const GLfloat bgTextureVertices[] = { 0, 0, w, 0, 0, h, w, h };
	const GLfloat bgTextureCoords[]   = { 1, 0, 1, 1, 0, 0, 0, 1 };
  	const GLfloat proj_bg[]           = { 0, -2.f/w, 0, 0, -2.f/h, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1 };
	


  	glMatrixMode(GL_PROJECTION);
  	glLoadMatrixf(proj_bg);

  	glMatrixMode(GL_MODELVIEW);
 	glLoadIdentity();

  	glEnable(GL_TEXTURE_2D);
  	glBindTexture(GL_TEXTURE_2D, backgroundTextureID);

  	// Update attribute values.
  	glEnableClientState(GL_VERTEX_ARRAY);
  	glEnableClientState(GL_TEXTURE_COORD_ARRAY);

  	glVertexPointer(2, GL_FLOAT, 0, bgTextureVertices);
  	glTexCoordPointer(2, GL_FLOAT, 0, bgTextureCoords);

  	glColor4f(1,1,1,1);
  	glDrawArrays(GL_TRIANGLE_STRIP, 0, 4);

  	glDisableClientState(GL_VERTEX_ARRAY);
  	glDisableClientState(GL_TEXTURE_COORD_ARRAY);
  	glDisable(GL_TEXTURE_2D);



	//draw the cube according to the camera matrix 
	float len;
	if (s < 1)	len = s;
	if (1 <= s) len = 1;
	
	float near = 0.1f;
	float far = 100.f;

	float f_x = K.at<double>(0,0);
	float f_y = K.at<double>(1,1);
	float c_x = K.at<double>(0,2);
	float c_y = K.at<double>(1,2); 

	
	const GLfloat proj_camera[] = { -2.0f*f_x/w, 0, 0, 0,
							  0, 2.0f*f_y / h, 0, 0,
							  2.0*c_x / w -1.0, 2.0*c_y / h - 1.0, -(far+near)/(far-near), -1.0,
							  0.0, 0.0, -2.0*far*near/(far-near), 1 };
	
 
	glMatrixMode(GL_PROJECTION);
    glLoadIdentity();
  	glMultMatrixf(proj_camera);

	//PnP method
	Mat r0 = RT.col(0);
	Mat r1 = RT.col(1);
	Mat r2 = RT.col(2);
	Mat t = RT.col(3);
  	

	const GLfloat RT_Camera[] = { r0.at<double>(0), r1.at<double>(0), r2.at<double>(0), 0,
								  -r0.at<double>(1), -r1.at<double>(1), -r2.at<double>(1), 0,
								  -r0.at<double>(2), -r1.at<double>(2), -r2.at<double>(2), 0,
						    	   t.at<double>(0),  t.at<double>(1),  t.at<double>(2), 1 };

	glMatrixMode(GL_MODELVIEW);
    glLoadIdentity();
	glMultMatrixf(RT_Camera);

	len = len/2;

	

    glColor4f(1.0f, 1.0f, 1.0f, 0.2f);
	//draw the cube faces
	glPolygonMode(GL_FRONT_AND_BACK,GL_FILL);
    glBegin(GL_QUADS);

	//left
	glVertex3f(0, 0, 0);
	glVertex3f(0, 0, len);
	glVertex3f(0, len, len);
	glVertex3f(0, len, 0);

	//right
	glVertex3f(-len, 0, len);
	glVertex3f(-len, len, len);
	glVertex3f(-len, len, 0);
	glVertex3f(-len, 0, 0);
	
	//top
	glVertex3f(0, 0, len);
	glVertex3f(0, len, len);
	glVertex3f(-len, len, len);
	glVertex3f(-len, 0, len);

	//back
	//glColor3f(0.0f, 0.0f, 0.0f);
	glVertex3f(0, len, 0);
	glVertex3f(0, len, len);
	glVertex3f(-len, len, len);
	glVertex3f(-len, len, 0);
	
	//front
	//glColor3f(0.f,0.f,1.0f);
	glVertex3f(0, 0, 0);
	glVertex3f(-len, 0, 0);
	glVertex3f(-len, 0, len);
	glVertex3f(0, 0, len);

	//bottom
    glVertex3f(   -len,   len,   0); //bottomrightback
    glVertex3f(   0,    len,   0); //bottomleftback
    glVertex3f(   0,    0,   0); //bottomleftfront
    glVertex3f(   -len,   0,   0); //bottomrightfront
	glEnd();


	//draw edges of cube
	glPolygonMode(GL_FRONT_AND_BACK,GL_LINE);
	glColor3f(0.0f, 0.0f, 0.0f); 
	glBegin(GL_QUADS);

	// 0 = -len/4, len = len/4
	//left
	glVertex3f(0, 0, 0);
	glVertex3f(0, 0, len);
	glVertex3f(0, len, len);
	glVertex3f(0, len, 0);

	//right
	glVertex3f(-len, 0, len);
	glVertex3f(-len, len, len);
	glVertex3f(-len, len, 0);
	glVertex3f(-len, 0, 0);
	
	//top
	glVertex3f(0, 0, len);
	glVertex3f(0, len, len);
	glVertex3f(-len, len, len);
	glVertex3f(-len, 0, len);

	//back
	//glColor3f(0.0f, 0.0f, 0.0f);
	glVertex3f(0, len, 0);
	glVertex3f(0, len, len);
	glVertex3f(-len, len, len);
	glVertex3f(-len, len, 0);
	
	//front
	//glColor3f(0.f,0.f,1.0f);
	glVertex3f(0, 0, 0);
	glVertex3f(-len, 0, 0);
	glVertex3f(-len, 0, len);
	glVertex3f(0, 0, len);

	//bottom
	//glColor3f(0.0f, 1.0f, 0.0f);
    glVertex3f(   -len,   len,   0); //bottomrightback
    glVertex3f(   0,    len,   0); //bottomleftback
    glVertex3f(   0,    0,   0); //bottomleftfront
    glVertex3f(   -len,   0,   0); //bottomrightfront
	glEnd();
	

	glColor3f(1.0f, 1.0f, 1.0f);
	glPolygonMode(GL_FRONT_AND_BACK,GL_FILL);





}

int main(int argc, char* argv[])
{

	glViewport(0,0, 640, 480);

	//background texture (webcam image) is not created until
	//webcam is initialized and displayed
	texture_created = 0;
	
	/* Enable smooth shading */
    glShadeModel( GL_SMOOTH );

    /* Set the background black */
    glClearColor( 0.0f, 0.0f, 0.0f, 0.0f );

    /* Depth buffer setup */
    glClearDepth( 1.0f );

    /* Enables Depth Testing */
    glEnable( GL_DEPTH_TEST );

    /* The Type Of Depth Test To Do */
    glDepthFunc( GL_LEQUAL );

    /* Really Nice Perspective Calculations */
    glHint( GL_PERSPECTIVE_CORRECTION_HINT, GL_NICEST );
	

	Mat image_next;
	VideoCapture cap(0);
	if(!cap.isOpened()) return -1;


	clicks = 0;
	cap >> image;
	cout << "rows: " << image.rows << " cols: " << image.cols << endl;
	namedWindow( "Click Points", CV_WINDOW_AUTOSIZE);
	
	imshow("Click Points", image);	
	setMouseCallback("Click Points", onMouse2, 0);

	//check if 4 points have been clicked to identify the plane 
	int points_clicked = 0;
	
	int check = 1;


	sf::RenderWindow App(sf::VideoMode(image.cols, image.rows, 32), "SFML window");
    App.Clear(sf::Color(255,0,0));	

	cout << "Camera Calibration" << endl;
	Size pattern(7,7);


	
    Mat cameraMatrix = Mat::eye(3, 3, CV_64F);
	Mat distCoeffs = Mat::zeros(8, 1, CV_64F);
	double rpe;
	//calibrate camera if given a 2nd argument to commandline
    if (argc > 1)
	{ 
		//calibrate camera
		vector<vector<Point3f> > object_points;
	
		vector<Point3f> obj;
		for (int x = 0; x < pattern.height; x++)
		{
			for (int y = 0; y < pattern.width; y++)
			{
				obj.push_back(Point3f(x,y,0));
			}
		}
		int num_images = 0;
		vector<vector<Point2f> > image_points;
		vector<Point2f> corners;
		double t = (double)getTickCount();
		double t2;
		while (check)
		{
			cap >> image;
			char key = waitKey(30);
			switch(key) {
			case 27:
				cout << "ESC was pressed" << endl;
				if (num_images > 0) check = 0;
				break;
			case 'p':
				if (findChessboardCorners(image, pattern, corners))
				{
					drawChessboardCorners(image, pattern, corners, true);
					image_points.push_back(corners);
					object_points.push_back(obj);
					num_images++;
				}
				else
					drawChessboardCorners(image, pattern, corners, false);
				imshow("Click Points", image);
		    	waitKey(1000);	
			
			default:
				imshow("Click Points", image);
			}
	    
		}
    
		vector<Mat> rvecs, tvecs;
		cout << cameraMatrix << endl;
		cout << distCoeffs << endl;
		double rpe = calibrateCamera(object_points, image_points, image.size(), cameraMatrix, distCoeffs,
								 rvecs, tvecs);

		cout << "Camera Matrix:" << endl << cameraMatrix << endl;
		cout << "distCoeffs:" << endl << distCoeffs << endl;
		cout << "reprojection error: " << rpe << endl;
	}
	//default camera matrix for my webcam
	else
	{
		cameraMatrix.at<double>(0,0) = 805.681948728115;
		cameraMatrix.at<double>(0,2) = 322.6647811002137;
		cameraMatrix.at<double>(1,1) = 805.3642730837905;
		cameraMatrix.at<double>(1,2) = 231.1740947838633;
	    rpe = 0.273059;
		distCoeffs.at<double>(0) = 0.04757978134005677;
		distCoeffs.at<double>(1) = -0.5350403941017606;
		distCoeffs.at<double>(2) = 0.000850515369893481;
		distCoeffs.at<double>(3) = -0.001186128332274569;
		distCoeffs.at<double>(4) = 2.20855766783285;
	}


	//Indicate the 4 points of the plane to put objects on!
	check = 1;
	cout << "Click 4 points of your plane in this order: bottom left \n top left \n" 
		 << "top right \n bottom right" << endl;
    //click 4 points
    while (check)
	{
		cap >> image;
		char key = waitKey(30);

		switch(key) {
		case 27:
			cout << "ESC was pressed" << endl;
			check = 0;
            break;
		case 'p':
			if (points_clicked)
				break;	
			clicks = 0;	
			setMouseCallback("Click Points", onMouse, 0);
			while (clicks < 4)
			{
				imshow("Click Points", image);
				waitKey(1);
			}
			setMouseCallback("Click Points", onMouse2, 0);
			points_clicked = 1;
			check = 0;
		default:
			imshow("Click Points", image);
		}
	}

	//Calculate the homography from the world coordinates 
	// [(0,0),(1,0),(1,s),(0,s)] to the pixel coords that you selected
	cout << "ITS HOMOGRAPHY TIME!" << endl;
	
	Mat H_wi;
	vector<Point2f> unit_square(4);
	unit_square[0] = Point(0,0);
	unit_square[1] = Point(0,1);
	unit_square[2] = Point(1,1);
	unit_square[3] = Point(1,0);

	//find homography between unit_square and im0_corners
	Mat H = findHomography(unit_square, im0_corners);
	//cout << "Homography: " << H << endl;
	Mat K_inv, KM;
	invert(cameraMatrix, K_inv);
	KM = K_inv*H;

	double h2 = pow(KM.at<double>(0,1),2) + pow(KM.at<double>(1,1),2) +	pow(KM.at<double>(2,1),2);
	double h1 = pow(KM.at<double>(0,0),2) + pow(KM.at<double>(1,0),2) +	pow(KM.at<double>(2,0),2);
	double s = h2 / h1;

	cout << "s = " << s << endl;
	
    Mat scale = Mat::eye(3, 3, CV_64F);
	scale.at<double>(1,1) = 1 / s;

	cout << "detect features points" << endl;
	H_wi = H*scale;

	//test H_w0
    Mat mp0 = (Mat_<double>(3,1) << 0, 0, 1);
	Mat mp1 = (Mat_<double>(3,1) << 0, s, 1);
	Mat mp2 = (Mat_<double>(3,1) << 1, s, 1);
	Mat mp3 = (Mat_<double>(3,1) << 1, 0, 1);

	Point p0 = transform_corner(H_wi, mp0);
	Point p1 = transform_corner(H_wi, mp1);
	Point p2 = transform_corner(H_wi, mp2);
	Point p3 = transform_corner(H_wi, mp3);


	cout << "Testing H_w0..." << endl;
	circle(image, p0, 4, Scalar(255,255,255), -1); 
	circle(image, p1, 4, Scalar(255,255,0), -1); 
	circle(image, p2, 4, Scalar(0,0,0), -1); 
	circle(image, p3, 4, Scalar(255,0,255), -1);
	imshow("Click Points", image);
	cout << "H_w0*p:" << endl
		 << "(x,y)" << endl 
		 << "(" << p0.x << "," << p0.y << ")" << endl
		 << "(" << p1.x << "," << p1.y << ")" << endl
		 << "(" << p2.x << "," << p2.y << ")" << endl
		 << "(" << p3.x << "," << p3.y << ")" << endl;
	

	
	//use obj_points and img_points to calculate the extrinsic camera matrix!
	//by using solvePnP!
	vector<Point3f> obj_points(4);
	obj_points[0] = Point3f(0.0f,0.0f,0.0f);
	obj_points[1] = Point3f(0.0f,s,0.0f);
	obj_points[2] = Point3f(1.0f,s,0.0f);
	obj_points[3] = Point3f(1.0f,0.0f,0.0f);

	vector<Point2f> img_points(4);
	img_points[0] = p0;
	img_points[1] = p1;
	img_points[2] = p2;
	img_points[3] = p3;

	Mat rvec, tvec, rmat;
	solvePnP(obj_points, img_points, cameraMatrix, distCoeffs, rvec, tvec);	
	Rodrigues(rvec,rmat);
	//tvec is the translation vector, rmat is the 3x3 rotation matrix

	Mat RT_pnp = Mat::eye(4, 4, CV_64F);
	rmat.copyTo(RT_pnp(Rect(0, 0, 3, 3)));
	tvec.copyTo(RT_pnp(Rect(3, 0, 1, 3)));

	Mat RT_pnp_inv = Mat::eye(4, 4, CV_64F);
	//since we are using opengl and going from
	//the camera to the object coordinates, we must invert our extrinsic matrix
	for (int i = 0; i < 3; i++)
	{
		for (int j = 0; j < 3; j++)
		{
			RT_pnp_inv.at<double>(i,j) = RT_pnp.at<double>(j,i);
		}
	}
	RT_pnp_inv.at<double>(0,3) = -1*RT_pnp.at<double>(0,3);
	RT_pnp_inv.at<double>(1,3) = -1*RT_pnp.at<double>(1,3);
	RT_pnp_inv.at<double>(2,3) = -1*RT_pnp.at<double>(2,3);
	
    
	updateSFML( 0, image, cameraMatrix, RT_pnp_inv, s);
	App.Display();

	waitKey(0);
	destroyWindow("Click Points");	
	

	Ptr<FeatureDetector> detector = Ptr<FeatureDetector>(new SurfFeatureDetector(500,4));
    vector<KeyPoint> keypoints_0, keypoints_next;
	detector->detect(image, keypoints_0);

	//Ptr<DescriptorExtractor> extractor = Ptr<DescriptorExtractor>(new SurfDescriptorExtractor());
    Ptr<DescriptorExtractor> extractor = Ptr<DescriptorExtractor>(new ORB());

	Mat descriptors_0, descriptors_next;
	extractor->compute(image, keypoints_0, descriptors_0);

	//Ptr<DescriptorMatcher> matcher = Ptr<DescriptorMatcher>(new BFMatcher(NORM_L2, true));
	Ptr<DescriptorMatcher> matcher = Ptr<DescriptorMatcher>(new BFMatcher(NORM_HAMMING, true));
	

	char key = 0;
	Mat H_ii1, H_wi1;
	Point p0_1, p1_1, p2_1, p3_1;
	check = 1;
    
	Mat drawn_image;
	Mat RT;

	Mat RT_pnp1 = Mat::eye(4, 4, CV_64F);
	Mat diff = Mat::zeros(4, 4, CV_64F);

	Mat rvec1, tvec1, rmat1;	
	vector<Point2f> img_points1(4);
	Mat RT_pnp1_inv = Mat::eye(4, 4, CV_64F);
	Mat diff_inv = Mat::eye(4, 4, CV_64F);

	//Steady state: get the next image from the webcam,
	//find the homography from the previous image to this next image
	//then find the correct extrinsic camera matrix and place a 3d
	//object into the scene
	while (check)
	{
		key = waitKey(1);
		switch(key) {
		case 'q':
			check = 0;
			break;
		default:
			break;
		}
		cap >> image_next;
		drawn_image = image_next;
		H_ii1 = find_next_homography(image, image_next, keypoints_0, descriptors_0,
						detector, extractor, matcher, keypoints_next, descriptors_next);
		H_wi1 = H_ii1 * H_wi;
	
		p0_1 = transform_corner(H_wi1, mp0);
		p1_1 = transform_corner(H_wi1, mp1);
		p2_1 = transform_corner(H_wi1, mp2);
		p3_1 = transform_corner(H_wi1, mp3);

		img_points1[0] = p0_1;
		img_points1[1] = p1_1;
		img_points1[2] = p2_1;
		img_points1[3] = p3_1;

	
		solvePnP(obj_points, img_points1, cameraMatrix, distCoeffs, rvec1, tvec1);	
		Rodrigues(rvec1,rmat1);
	
		rmat1.copyTo(RT_pnp1(Rect(0, 0, 3, 3)));
		tvec1.copyTo(RT_pnp1(Rect(3, 0, 1, 3)));

		cout << "RT_pnp1: " << RT_pnp1 << endl;

			
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				RT_pnp1_inv.at<double>(i,j) = RT_pnp1.at<double>(j,i);
			}
		}
		RT_pnp1_inv.at<double>(0,3) = -1*RT_pnp1.at<double>(0,3);
		RT_pnp1_inv.at<double>(1,3) = -1*RT_pnp1.at<double>(1,3);
		RT_pnp1_inv.at<double>(2,3) = -1*RT_pnp1.at<double>(2,3);

		drawPlane(drawn_image, p0_1, p1_1, p2_1, p3_1);
		//imshow("H_ii1", drawn_image);
		keypoints_0 = keypoints_next;
		descriptors_0 = descriptors_next;
		image = image_next; 
		H_wi = H_wi1;
		updateSFML( 0, image, cameraMatrix, RT_pnp1_inv, s);
		App.Display();

	}
	
	waitKey(0);
	return 0;

}

