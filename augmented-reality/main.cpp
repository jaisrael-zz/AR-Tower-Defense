#include <opencv2/highgui/highgui.hpp>
#include <opencv2/core/core.hpp>
#include <opencv2/calib3d/calib3d.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/features2d/features2d.hpp>
#include <opencv2/nonfree/features2d.hpp>
#include <opencv2/opencv.hpp>
//#include <GL/glew.h>
//#include <GL/gl.h>
//#include <GL/glu.h>
#include <SFML/Window.hpp>
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
	Point center = Point(x,y);
	im0_corners[clicks] = center; 
	//circle(image, center, 4, color, -1); 
    cout << "(" << x << "," << y << ")" << endl;
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

Mat find_next_homography(Mat im, Mat image_next, vector<KeyPoint> keypoints_0, Mat descriptors_0,
						 Ptr<FeatureDetector> detector, Ptr<DescriptorExtractor> extractor, 
						 Ptr<DescriptorMatcher> matcher, vector<KeyPoint>& keypoints_next, 
						 Mat& descriptors_next)
{

	//step 1 detect feature points in next image
	vector<KeyPoint> keypoints_1;
	detector->detect(image_next, keypoints_1);

	Mat img_keypoints_surf0, img_keypoints_surf1;
	//drawKeypoints(im, keypoints_0, img_keypoints_surf0);
	//drawKeypoints(image_next, keypoints_1, img_keypoints_surf1);
	//cout << "# im0 keypoints" << keypoints_0.size() << endl;
    //cout << "# im1 keypoints" << keypoints_1.size() << endl;
	//imshow("surf 0", img_keypoints_surf0);
	//imshow("surf 1", img_keypoints_surf1);

    //step 2: extract feature descriptors from feature points
	Mat descriptors_1;
	extractor->compute(image_next, keypoints_1, descriptors_1);

	//step 3: feature matching
	//cout << "fd matching" << endl;
	vector<DMatch> matches;
	vector< vector<DMatch> > k_matches;
	vector<Point2f> matched_0;
	vector<Point2f> matched_1;

	/*cout << "test1" << endl;	
	matcher->knnMatch(descriptors_0, descriptors_1, k_matches, 1);

	float threshold = 1.5f/ 1.0f;

	cout << "size kmatches: " << k_matches.size() << endl;	
	for (int i = 0; i < k_matches.size(); i++)
	{
		DMatch first_choice = k_matches[i][0];
		//DMatch& second_choice = k_matches[i][1];
		//float ratio = first_choice.distance / second_choice.distance;
		//if (ratio >= threshold)
		//matches.push_back(first_choice);

	}

	cout << "test3" << endl;	
*/
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

void updateSFML(float elapsedTime, Mat backgroundImage, Mat K, Mat H, double s)
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
  	const GLfloat proj_bg[]              = { 0, -2.f/w, 0, 0, -2.f/h, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1 };

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
	if (1 >= s) len = 1;
	
	float near = 0.1;
	float far = 100.f;

	//build projection matrix from K
	float f_x = K.data[0];
	float f_y = K.data[4];
	//float f_z = K.data[4];  //maybe its f_z?
	float c_x = K.data[2];
	float c_y = K.data[5]; 
	//float c_z = K.data[5]; i

	//remember this is in column major order!!!!
	//so its actually the transpose of what it looks like!
  	const GLfloat proj_camera[] = { -2.f*f_x / w, 0, 0, 0, 
								    0, -2.f*f_y/ h, 0, 0, 
								  2.f*c_x / w - 1, 2.f*c_y / h - 1, -(far+near)/ (far - near), -1.f, 
								  0, 0, -2.f*far*near / (far - near), 1 };
	
    glMatrixMode(GL_PROJECTION);
    glLoadIdentity();
  	//glLoadMatrixf(proj_bg);
    //fov, aspect ratio, nearclip, farclip
    gluPerspective(90.f, 1.f, 1.f, 500.f);
    
	glMatrixMode(GL_MODELVIEW);
    glLoadIdentity();
    glTranslatef(0.f, 0.f, -200.f);
    glRotatef(elapsedTime*50, 1.f, 0.f, 0.f);
    glRotatef(elapsedTime*30, 0.f, 1.f, 0.f);
    glRotatef(elapsedTime*90, 0.f, 0.f, 1.f);

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

	

}

int main(int argc, char* argv[])
{
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

	//cap.set(CV_CAP_PROP_FRAME_HEIGHT, 240);	
	//cap.set(CV_CAP_PROP_FRAME_WIDTH, 320);	

	clicks = 0;
	cap >> image;
	cout << "rows: " << image.rows << " cols: " << image.cols << endl;
	//namedWindow( "Click Points", CV_WINDOW_AUTOSIZE);
	
	imshow("Click Points", image);	
	setMouseCallback("Click Points", onMouse2, 0);

	int points_clicked = 0;
	int check = 1;


	sf::Clock Clock;
	sf::Window App(sf::VideoMode(image.cols, image.rows, 32), "SFML window");
	

	//imshow("Click Points", image);
	cout << "Camera Calibration" << endl;
	Size pattern(7,7);

    
	//calibrate camera
	/*
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
			//check = 0;
			imshow("Click Points", image);
		    waitKey(1000);	
			
		default:
			imshow("Click Points", image);
		}
	    
	}
    
    Mat cameraMatrix = Mat::eye(3, 3, CV_64F);
	Mat distCoeffs = Mat::zeros(8, 1, CV_64F);
	vector<Mat> rvecs, tvecs;
	cout << cameraMatrix << endl;
	cout << distCoeffs << endl;
	double rpe = calibrateCamera(object_points, image_points, image.size(), cameraMatrix, distCoeffs,
								 rvecs, tvecs);

	cout << "Camera Matrix:" << endl << cameraMatrix << endl;
	cout << "distCoeffs:" << endl << distCoeffs << endl;
	cout << "reprojection error: " << rpe << endl;
    */
	Mat cameraMatrix = Mat::eye(3, 3, CV_64F);
	Mat distCoeffs = Mat::zeros(8, 1, CV_64F);
	cameraMatrix.at<double>(0,0) = 805.681948728115;
	cameraMatrix.at<double>(0,2) = 322.6647811002137;
	cameraMatrix.at<double>(1,1) = 805.3642730837905;
	cameraMatrix.at<double>(1,2) = 231.1740947838633;
	double rpe = 0.273059;

	check = 1;
	cout << "Click 4 points of your plane" << endl;
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


	cout << "ITS HOMOGRAPHY TIME!" << endl;
	//H_w^0 homography
	
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
	//H_wi = H*scale;
	H_wi = KM*scale;

	//test H_w0
    Mat mp0 = (Mat_<double>(3,1) << 0, 0, 1);
	Mat mp1 = (Mat_<double>(3,1) << 0, s, 1);
	Mat mp2 = (Mat_<double>(3,1) << 1, s, 1);
	Mat mp3 = (Mat_<double>(3,1) << 1, 0, 1);

	Point p0 = transform_corner(H_wi, mp0);
	Point p1 = transform_corner(H_wi, mp1);
	Point p2 = transform_corner(H_wi, mp2);
	Point p3 = transform_corner(H_wi, mp3);

/*
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
*/

/*	vector<Point2f> rect_corners(4);
	rect_corners[0] = Point(0,500*s); //bottom left
	rect_corners[1] = Point(0,0); //top left
	rect_corners[2] = Point(500, 0); //top right
	rect_corners[3] = Point(500, 500*s); //bottom left

	Mat rect_homography = findHomography(im0_corners,rect_corners);
	cout << "rect homo " << rect_homography << endl;
	Point c0 = transform_corner(rect_homography, (Mat_<double>(3,1) << 0, 0, 1));
	Point c1 = transform_corner(rect_homography, (Mat_<double>(3,1) << image.rows-1, 0, 1));
	Point c2 = transform_corner(rect_homography, (Mat_<double>(3,1) << image.rows-1, image.cols-1, 1));
	Point c3 = transform_corner(rect_homography, (Mat_<double>(3,1) << 0, image.cols-1, 1));
	

	cout << "c0 " << c0 << endl
		 << "c1 " << c1 << endl
		 << "c2 " << c2 << endl
		 << "c3 " << c3 << endl;

	float y_values[] {c0.y, c1.y, c2.y, c3.y};
	float x_values[] {c0.x, c1.x, c2.x, c3.x};
	int min_r = floor(*min_element(y_values, y_values+3));
	int max_r = ceil(*max_element(y_values, y_values+3));
	int min_c = floor(*min_element(x_values, x_values+3));
	int max_c = ceil(*max_element(x_values, x_values+3));

	cout << "minr " << min_r << endl
		 << "maxr " << max_r << endl
		 << "minc " << min_c << endl
		 << "maxc " << max_c << endl;

	Mat rectified_plane(500, 500*s, CV_64FC3);
	warpPerspective(image, rectified_plane, rect_homography, Size(500, 500*s));
	
	imshow("Rectify", rectified_plane);	
*/

	waitKey(0);
	destroyWindow("Click Points");	
	//GoodFeaturesToTrackDetector detector(500, 0.01, 1, 3, true, 0.04);
	Ptr<FeatureDetector> detector = Ptr<FeatureDetector>(new SurfFeatureDetector(500,4));
	//Ptr<FeatureDetector> detector = Algorithm::create<FeatureDetector>("Feature2D.BRISK");
	//Ptr<FeatureDetector> detector = Ptr<FeatureDetector>(new FastFeatureDetector());
	//Ptr<FeatureDetector> detector = Ptr<FeatureDetector>(new ORB());
    vector<KeyPoint> keypoints_0, keypoints_next;
	detector->detect(image, keypoints_0);
	//detector->detect(rectified_plane, keypoints_0);

	//Ptr<DescriptorExtractor> extractor = Ptr<DescriptorExtractor>(new SurfDescriptorExtractor());
	//Ptr<DescriptorExtractor> extractor = Ptr<BriskExtractor>(new BriskDescriptorExtractor());
	Ptr<DescriptorExtractor> extractor = Ptr<DescriptorExtractor>(new ORB());
    //Ptr<DescriptorExtractor> extractor = Ptr<DescriptorExtractor>(new FREAK());

	Mat descriptors_0, descriptors_next;
	extractor->compute(image, keypoints_0, descriptors_0);
	//extractor->compute(rectified_plane, keypoints_0, descriptors_0);

	//FlannBasedMatcher matcher;
	//Ptr<DescriptorMatcher> matcher = Ptr<DescriptorMatcher>(new BFMatcher(NORM_L2, true));
	Ptr<DescriptorMatcher> matcher = Ptr<DescriptorMatcher>(new BFMatcher(NORM_HAMMING, true));
	//Ptr<DescriptorMatcher> matcher = Ptr<DescriptorMatcher>(new FlannBasedMatcher());
	//BFMatcher matcher(NORM_HAMMING, true);
	char key = 0;
	Mat H_ii1, H_wi1;
	Point p0_1, p1_1, p2_1, p3_1;
	check = 1;
    
    Mat rp0 = (Mat_<double>(3,1) << 0, 0, 1);
	Mat rp1 = (Mat_<double>(3,1) << 0, 500*s, 1);
	Mat rp2 = (Mat_<double>(3,1) << 1*500, 500*s, 1);
	Mat rp3 = (Mat_<double>(3,1) << 500*1, 0, 1);
/*	
	while (check)
	{
		key = waitKey(1);
		switch(key) {
		case 'q':
			check = 0;
			break;
		case 'f':
			cap >> image_next;
			H_ii1 = find_next_homography(rectified_plane, image_next, keypoints_0, descriptors_0,
							detector, extractor, matcher, keypoints_next, descriptors_next);
				
			p0_1 = transform_corner(H_ii1, rp0);
			p1_1 = transform_corner(H_ii1, rp1);
			p2_1 = transform_corner(H_ii1, rp2);
			p3_1 = transform_corner(H_ii1, rp3);
			
			drawPlane(image_next, p0_1, p1_1, p2_1, p3_1);
			imshow("H_ii1", image_next);
			default:
			break;
		}
		if (argc == 1)
		{
			cap >> image_next;
			H_ii1 = find_next_homography(image, image_next, keypoints_0, descriptors_0,
							detector, extractor, matcher, keypoints_next, descriptors_next);
			H_wi1 = H_ii1 * H_wi;
			p0_1 = transform_corner(H_wi1, mp0);
			p1_1 = transform_corner(H_wi1, mp1);
			p2_1 = transform_corner(H_wi1, mp2);
			p3_1 = transform_corner(H_wi1, mp3);
			
			drawPlane(image_next, p0_1, p1_1, p2_1, p3_1);
			imshow("H_ii1", image_next);
			keypoints_0 = keypoints_next;
			descriptors_0 = descriptors_next;
			image = image_next; 
			H_wi = H_wi1;
		}
		 
	}
*/
    cvtColor(image, image, CV_RGB2GRAY);
	Mat drawn_image;
	H_wi = cameraMatrix*H_wi;  //K*H
	while (check)
	{
		float elapsedTime = Clock.GetElapsedTime();
		key = waitKey(1);
		switch(key) {
		case 'q':
			check = 0;
			break;
		case 'f':
			cap >> image_next;
    		cvtColor(image_next, image_next, CV_RGB2GRAY);	
			H_ii1 = find_next_homography(image, image_next, keypoints_0, descriptors_0,
							detector, extractor, matcher, keypoints_next, descriptors_next);
			H_wi1 = H_ii1 * H_wi;
			p0_1 = transform_corner(H_wi1, mp0);
			p1_1 = transform_corner(H_wi1, mp1);
			p2_1 = transform_corner(H_wi1, mp2);
			p3_1 = transform_corner(H_wi1, mp3);
			
			drawPlane(image_next, p0_1, p1_1, p2_1, p3_1);
			imshow("H_ii1", image_next);
			keypoints_0 = keypoints_next;
			descriptors_0 = descriptors_next;
			image = image_next; 
			H_wi = H_wi1;
		default:
			break;
		}
		if (argc == 1)
		{
			cap >> image_next;
			drawn_image = image_next;
			H_ii1 = find_next_homography(image, image_next, keypoints_0, descriptors_0,
							detector, extractor, matcher, keypoints_next, descriptors_next);
			H_wi1 = H_ii1 * H_wi;
			
			p0_1 = transform_corner(H_wi1, mp0);
			p1_1 = transform_corner(H_wi1, mp1);
			p2_1 = transform_corner(H_wi1, mp2);
			p3_1 = transform_corner(H_wi1, mp3);
			
			drawPlane(drawn_image, p0_1, p1_1, p2_1, p3_1);
			imshow("H_ii1", drawn_image);
			keypoints_0 = keypoints_next;
			descriptors_0 = descriptors_next;
			image = image_next; 
			H_wi = H_wi1;
			updateSFML( elapsedTime, image, cameraMatrix, H_wi, s);
			App.Display();
		}
		 
	}
	
	waitKey(0);
	return 0;

}


