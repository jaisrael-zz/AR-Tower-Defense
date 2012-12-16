#include <opencv2/highgui/highgui.hpp>
#include <opencv2/core/core.hpp>
#include <opencv2/calib3d/calib3d.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/features2d/features2d.hpp>
#include <opencv2/nonfree/features2d.hpp>
#include <opencv2/opencv.hpp>
#include <iostream>
#include "math.h"

using namespace cv;
using namespace std;

int clicks;
Mat image;
vector<Point2f> im0_corners(4); 

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
	circle(image, center, 4, color, -1); 
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
	circle(image, p0, 4, Scalar(0,0,255), -1); 
	circle(image, p1, 4, Scalar(0,255,0), -1); 
	circle(image, p2, 4, Scalar(255,0,0), -1); 
	circle(image, p3, 4, Scalar(0,255,255), -1); 

}

Mat find_next_homography(Mat image, Mat image_next, vector<KeyPoint> keypoints_0, Mat descriptors_0,
						 SurfFeatureDetector detector, SurfDescriptorExtractor extractor, 
						 BFMatcher matcher, vector<KeyPoint>& keypoints_next, Mat& descriptors_next)
{

	//step 1 detect feature points in next image
	vector<KeyPoint> keypoints_1;
	detector.detect(image_next, keypoints_1);

	Mat img_keypoints_surf0, img_keypoints_surf1;
	drawKeypoints(image, keypoints_0, img_keypoints_surf0);
	drawKeypoints(image_next, keypoints_1, img_keypoints_surf1);
	//cout << "# im0 keypoints" << keypoints_0.size() << endl;
    //cout << "# im1 keypoints" << keypoints_1.size() << endl;
	imshow("surf 0", img_keypoints_surf0);
	imshow("surf 1", img_keypoints_surf1);

    //step 2: extract feature descriptors from feature points
	Mat descriptors_1;
	extractor.compute(image_next, keypoints_1, descriptors_1);

	//step 3: feature matching
	//cout << "fd matching" << endl;
	vector<DMatch> matches;
	vector<Point2f> matched_0;
	vector<Point2f> matched_1;

	matcher.match(descriptors_0, descriptors_1, matches);
	Mat img_feature_matches;
	drawMatches(image, keypoints_0, image_next, keypoints_1, matches, img_feature_matches );
	imshow("Matches", img_feature_matches);

	for (int i = 0; i < matches.size(); i++ )
	{
		matched_0.push_back(keypoints_0[matches[i].queryIdx].pt);	
		matched_1.push_back(keypoints_1[matches[i].trainIdx].pt);	
	}
	keypoints_next = keypoints_1;
	descriptors_next = descriptors_1;
	return findHomography(matched_0, matched_1, RANSAC);

}
int main(int argc, char* argv[])
{
	Mat image_next;
	VideoCapture cap(0);
	if(!cap.isOpened()) return -1;

	//cap.set(CV_CAP_PROP_FRAME_HEIGHT, 240);	
	//cap.set(CV_CAP_PROP_FRAME_WIDTH, 320);	

	clicks = 0;
	cap >> image;
	cout << "rows: " << image.rows << " cols: " << image.cols << endl;
	namedWindow( "Click Points", CV_WINDOW_AUTOSIZE);
	imshow("Click Points", image);	

	setMouseCallback("Click Points", onMouse2, 0);

	int points_clicked = 0;
	int check = 1;

	

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
	check = 1;

	
	//GoodFeaturesToTrackDetector detector(500, 0.01, 1, 3, true, 0.04);
	SurfFeatureDetector detector(400);
    vector<KeyPoint> keypoints_0, keypoints_next;
	detector.detect(image, keypoints_0);

	//BriefDescriptorExtractor extractor;
	//FREAK* extractor = new FREAK();
	SurfDescriptorExtractor extractor;

	Mat descriptors_0, descriptors_next;
	extractor.compute(image, keypoints_0, descriptors_0);
	//extractor.compute(image, keypoints_0, descriptors_0);

	//FlannBasedMatcher matcher;
	BFMatcher matcher( NORM_L2, true);
	//BFMatcher matcher(NORM_HAMMING, true);
	char key = 0;
	Mat H_ii1, H_wi1;
	Point p0_1, p1_1, p2_1, p3_1;
	while (check)
	{
		key = waitKey(1);
		switch(key) {
		case 'q':
			check = 0;
			break;
		case 'f':
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
		default:
			break;
		}
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
	waitKey(0);
	return 0;

}


