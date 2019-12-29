#pragma once
#include "SVG_element.h"
#include "crossword.h"
#include "dictionary.h"
#include <thread>
#include <chrono>

//#define DEBUG

struct Fig {
	vector<pair<double, double>> points;

	string getSvgPath() {
		string path;
		path.push_back('M');
		for (int i = 0; i < points.size(); i++) {
			path += to_string(points[i].first);
			path += " ";
			path += to_string(points[i].second);
			path += (i == points.size() - 1 ? "Z" : "L");
		}
		return path;
	}
	double getWidth() {
		double maxX = points[0].first, minX = points[0].first;
		for (int i = 0; i < points.size(); i++) {
			maxX = max(maxX, points[i].first);
			minX = min(minX, points[i].first);
		}
		return maxX - minX;
	}
	double getHeight() {
		double maxY = points[0].second, minY = points[0].second;
		for (int i = 0; i < points.size(); i++) {
			maxY = max(maxY, points[i].second);
			minY = min(minY, points[i].second);
		}
		return maxY - minY;
	}
	pair<double, double> getCenter() {
		double maxX = points[0].first, maxY = points[0].second, minX = points[0].first, minY = points[0].second;
		for (int i = 0; i < points.size(); i++) {
			maxX = max(maxX, points[i].first);
			maxY = max(maxY, points[i].second);
			minX = min(minX, points[i].first);
			minY = min(minY, points[i].second);
		}
		return make_pair((maxX + minX) / 2, (maxY + minY) / 2);
	}
	void rotate(double angle, pair<double,double> center) {
		double pi = 3.141592653589793238463;
		auto rads = angle * pi / 180;

		for (int i = 0; i < points.size(); i++) {
			double x1 = points[i].first - center.first;
			double y1 = points[i].second - center.second;

			double temp_x1 = x1 * cos(rads) - y1 * sin(rads);
			double temp_y1 = x1 * sin(rads) + y1 * cos(rads);

			points[i] = make_pair(temp_x1 + center.first, temp_y1 + center.second);
		}
	}
	void scaleUniform(double x, double y) {
		auto center = getCenter();
		translate(-center.first, -center.second);
		for (int i = 0; i < points.size(); i++) {
			points[i].first *= x;
			points[i].second *= y;
		}
		translate(center.first, center.second);
	}
	void scale(double x, double y) {
		for (int i = 0; i < points.size(); i++) {
			points[i].first *= x;
			points[i].second *= y;
		}
	}
	void translate(double x, double y) {
		for (int i = 0; i < points.size(); i++) {
			points[i].first += x;
			points[i].second += y;
		}
	}
	void normalize() {
		double minX = points[0].first, minY = points[0].second;
		for (int i = 0; i < points.size(); i++) {
			minX = min(minX, points[i].first);
			minY = min(minY, points[i].second);
		}
		for (int i = 0; i < points.size(); i++) {
			points[i].first -= minX;
			points[i].second -= minY;
		}
	}
};

struct square {
	bool ForbidBot = false, ForbidTop = false;

	int topOccupied = 0;
	int botOccupied = 0;

	int has_anomaly = 0;

	int lines = 0;
	vector<pair<int, int>> word_clue_ind;
	vector<string> clues;

};

struct duma {
	string actualWord;
	int i, j, clueInd = 0, areaInd;
	bool hor;
	int anomaly = 0;
	vector<pair<string,int>> clue;
	vector<pair<int, int>> adjacent;
	pair<int, int> defaultSquare;
	vector<pair<string, int>> allClues;
};

class Smith
{
private:
	crossword cross;
	map<string, map<string,int>> alternatives;
	set<string> forHelpDict;
	vector<vector<square>> squares;
	vector<vector<square>> bestSol;
	vector<vector<int>> clusters;
	vector<vector<string>> NameClues;
	double bestBr = 100000;
	int bestSwap = 100000;
	double defaultSide;
	double arrowShrinkRay = 0.23;
	bool finishSearch = false, terminateThread = false;

	SVG::SVG_element ClueToSVG(string clue, string anchor, double,double,string,string);
	int getWordHeightType(int i, int j, duma &w);
	void count3Sec();
	void normalizeFigures();
	void placeClues(int m, int br);
	void placeClues_Clusters(int m, double br, int n);
	void createCluster(int m, vector<bool> &used, vector<int> &cluster);
	void loadAlternatives();
	void findAlternative(int i, int j);
	void parseDictAlternative();
	void loadAlternativeClues(duma& w);
	string to_upper(string s);
	string clearString(string s);
	wstring readFinishedSVG(string);
	pair<string, int> cutClue(string clue);
	void prepare(); 
	vector<Fig> loadFigures(string fileName);
	set<int> separate(std::string word);
	void recSplit(std::vector<std::string>& tokens, std::vector<std::set<int>>& separated, std::string currClue,
		int lines, int cuts, double len,
		std::string& bestClue, int& bestLines, int& bestCuts, double& bestSigma, int i = 0, int j = 0);

public:

	int setInd = 0, squareInd = 0, arrowInd = 2;
	double setRotation = 0, squareRotation = 0, setSizeCoefX = 1, setSizeCoefY = 1, squareSizeCoefX = 1.05, squareSizeCoefY = 1.05, arrowSizeCoefX = 1, arrowSizeCoefY = 1;
	string squareFill = "#FF000000", setFill = "#FFFFFFFF", arrowFill = "#FF000000";
	double squareOp, setOp, arrowOp;

	Smith();
	Smith(crossword cross);
	~Smith();

	dictionary dict;
	vector<duma> words;
	vector<Fig> boxes;
	vector<Fig> arrows;

	void drawCrossword();
	void setCrossword(crossword cross);
	static void replace(string& str, string from, string to);
	static pair<string,double> decodeColor(string c);
	static bool isValidLetter(uc c);
	static double calculateSD(vector<double> data);
	static vector<string> tokenize(string s);
	static vector<string> splitExplanation(string explanation);
	static vector<string> split(const string& str, const string& delim);
	void updateAlternative();
	void printAlternativesToFile();
	void printTxtAnswers();
};

