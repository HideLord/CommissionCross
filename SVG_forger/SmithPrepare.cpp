#include "Smith.h"

void Smith::count3Sec() {
	int br = 0;
	while (!terminateThread) {
		std::this_thread::sleep_for(std::chrono::milliseconds(30));
		br += 30;
		if(br > 1000)cout << br << "/" << 5000 << "\r";
		if (br >= 5000)break;
	}
	finishSearch = true;
}

void Smith::prepare()
{
	cout << "Preparing: " << endl;
	dict.loadDict();
	boxes = loadFigures("squares.txt");
	arrows = loadFigures("arrows.txt");
	normalizeFigures();

	loadAlternatives();
	parseDictAlternative();

	sort(cross.areas.begin(), cross.areas.end(), position::sortHelp);

	bool needsClear = false;

	cout << "Loading words: " << endl;
	int wPerSquare[400][400] = {0};
	for (int k = 0; k < cross.areas.size(); k++) {
		words.push_back(duma());
		words.back().areaInd = k;
		words.back().actualWord = to_upper(cross.areas[k].to_string());

		words.back().hor = cross.areas[k].hor;
		words.back().i = cross.areas[k].letters[0].second / cross.M;
		words.back().j = cross.areas[k].letters[0].second % cross.M;
		vector<string> clues;
		if (dict.explanationDict.count(cross.areas[k].to_string())) {
			clues = splitExplanation(dict.explanationDict[cross.areas[k].to_string()]);
		}

		for (int i = 0; i < clues.size(); i++) {
			words.back().clue.push_back(cutClue(clues[i]));
		}
		words.back().allClues = words.back().clue;
		for (int i = 0; i < words.back().clue.size(); i++) {
			// UNDER EXAMINATION! Not putting words in alternative dictionary because we only want the fixed by author words.
			// alternatives[words.back().actualWord][to_upper(words.back().clue[i].first)] = words.back().clue[i].second;
		}
#ifndef DEBUG
		if (words.back().clue.size() > 1) {
			cout << "actualwords :" <<  words.back().actualWord << endl;
			cout << "optionstart" << endl;
			for (int i = 0; i < words.back().clue.size(); i++) {
				cout << words.back().clue[i].first << "\n!!!\n";
			}
			cout << "optionend" << endl;
			int userChoice;
			cin >> userChoice;
			cout << "waitforme" << endl;
			needsClear = true;
			words.back().clue = vector<pair<string, int>>(1, words.back().clue[userChoice - 1]);
			words.back().clueInd = userChoice - 1;
		}
		if (words.back().clue.empty()) {
			cout << "The word: " << cross.areas[k].to_string() << " was not found in the dictionary! " << endl;
			
			cout << "custom " << cross.areas[k].to_string() << endl;
			string customExp;
			if (needsClear)needsClear = false, cin.ignore();
			getline(cin, customExp);
			cout << "waitforme" << endl;
			words.back().clue.push_back(cutClue(customExp));
		}
		else {
			loadAlternativeClues(words.back());
		}
#endif // DEBUG

		int i = cross.areas[k].letters[0].second / cross.M, j = cross.areas[k].letters[0].second % cross.M;
		if (cross.isNormalBox(i, j - 1))words.back().adjacent.push_back(make_pair(i, j - 1));
		if (cross.isNormalBox(i - 1, j))words.back().adjacent.push_back(make_pair(i - 1, j));
		if (cross.isNormalBox(i + 1, j))words.back().adjacent.push_back(make_pair(i + 1, j));
		if (cross.isNormalBox(i, j + 1))words.back().adjacent.push_back(make_pair(i, j + 1));

		{
			bool canUseBottom = false;
			for (int i = 0; i < words.back().adjacent.size(); i++) {
				wPerSquare[words.back().adjacent[i].first][words.back().adjacent[i].second]++;
				if (wPerSquare[words.back().adjacent[i].first][words.back().adjacent[i].second] >= 3 && words.back().adjacent.size()==1)canUseBottom = 1, cout << "can use bottom!" << endl;
			}
			int i = cross.areas[k].letters.back().second / cross.M, j = cross.areas[k].letters.back().second % cross.M;
			if (words.back().adjacent.empty() || canUseBottom) {
				if (!cross.areas[k].hor) {
					if (cross.isNormalBox(i - 1, j))words.back().adjacent.push_back(make_pair(i - 1, j));
					if (cross.isNormalBox(i + 1, j))words.back().adjacent.push_back(make_pair(i + 1, j));
				}
				else {
					if (cross.isNormalBox(i, j - 1))words.back().adjacent.push_back(make_pair(i, j - 1));
					if (cross.isNormalBox(i, j + 1))words.back().adjacent.push_back(make_pair(i, j + 1));
				}
			}
		}

		if (cross.areas[k].hor) {
			if (j == 0)
				words.back().defaultSquare = make_pair(i - 1, j);
			else
				words.back().defaultSquare = make_pair(i, j - 1);
		}
		else {
			if (i == 0)
				words.back().defaultSquare = make_pair(i, j - 1);
			else
				words.back().defaultSquare = make_pair(i - 1, j);
		}
		if (!cross.isNormalBox(words.back().defaultSquare.first, words.back().defaultSquare.second)) {
			words.back().anomaly = 1;
		}
		else {
			for (int i = 0; i < words.back().adjacent.size(); i++) {
				if (words.back().adjacent[i] == words.back().defaultSquare) {
					swap(words.back().adjacent[0], words.back().adjacent[i]);
					break;
				}
			}
		}
	}
	for (int i = 0; i < cross.N; i++) {
		squares.push_back(vector<square>());
		for (int j = 0; j < cross.M; j++) {
			squares.back().push_back(square());
		}
	}

	vector<bool> used(1000, 0);
	for (int i = 0; i < words.size(); i++) {
		if (used[i])continue;
		vector<int> cluster;
		createCluster(i, used, cluster);
		clusters.push_back(cluster);
	}

	cout << "Placing clues: " << endl;

	for (int cl = 0; cl < clusters.size(); cl++) {
		bestBr	 = 100000;
		bestSwap = 100000;
		terminateThread = false;
		finishSearch = false;
		thread timeCount(&Smith::count3Sec, this);
		placeClues_Clusters(0, 0, cl);
		terminateThread = true;
		timeCount.join();
		squares = bestSol;
		if (bestBr > 10000) {
			for (int i = 0; i < clusters[cl].size(); i++) {
				duma & w = words[clusters[cl][i]];
				square & sq = squares[w.defaultSquare.first][w.defaultSquare.second];
				sq.clues.push_back(w.clue[0].first);
				sq.word_clue_ind.push_back(make_pair(clusters[cl][i], 0));
				sq.lines += w.clue[0].second;
			}
		}
	}
	
	//placeClues(0, 0);
	squares = bestSol;
	int br = 0;
	for (int i = 0; i < cross.N; i++) {
		for (int j = 0; j < cross.M; j++) {
			if (!squares[i][j].clues.empty()) {
				if (squares[i][j].lines > 6) {
					findAlternative(i, j);
					br += (squares[i][j].lines > 6);
				}
			}
		}
	}
	std::cout << "Squares that still require fixing: " << br << endl;
}

void Smith::loadAlternatives()
{
	ifstream fin("Z:\\Cross\\altDict.txt");
	if (!fin.is_open()) {
		cout << "WARNING! could not open altDict.txt!" << endl;
		return;
	}
	string word, clue, line = "";
	int lineBr = 0;
	while (!fin.eof()) {
		lineBr = 0;
		char c;
		word = "";
		while (1) {
			fin.get(c);
			if (c == '\n')break;
			word.push_back(c);
			if (fin.eof())break;
		}
		if (fin.eof())break;

		clue = "";
		line = "";
		while (line != "!!!") {
			if (!line.empty() && line != "\n")clue += line, clue += "\n", lineBr++;
			if(!getline(fin, line))break;
		}
		if (!clue.empty() && clue.back() == '\n')clue.pop_back();
		if (clue.empty())continue;

		//cout << word << endl << clue << endl;

		alternatives[word][clue] = lineBr;
	}
	fin.close();
}

void Smith::findAlternative(int i, int j)
{
	square &sq = squares[i][j];
	if (sq.clues.size() != 2)return;
	duma &w1 = words[sq.word_clue_ind[0].first];
	duma &w2 = words[sq.word_clue_ind[1].first];
	bool shown = 0;
	string w1Actual = w1.actualWord;
	string w2Actual = w2.actualWord;
	vector<pair<pair<string,int>, pair<string,int>>> options;

	//find if there is an alternative of the chosen explanation
	
	vector<pair<string, int>> def1, def2;

	for (auto i : alternatives[w1Actual]) {
		int min1 = 10000000, ind;

		for (int j = 0; j < w1.allClues.size(); j++) {
			int dist = dictionary::levenstein(i.first, to_upper(w1.allClues[j].first));
			if (dist < min1) {
				ind = j;
				min1 = dist;
			}
		}

		if (ind == w1.clueInd) {
			def1.push_back(i);
		}
	}
	for (auto i : alternatives[w2Actual]) {
		int min2 = 10000000, ind;

		for (int j = 0; j < w2.allClues.size(); j++) {
			int dist = dictionary::levenstein(i.first, to_upper(w2.allClues[j].first));
			if (dist < min2) {
				ind = j;
				min2 = dist;
			}
		}

		if (ind == w2.clueInd) {
			def2.push_back(i);
		}
	}

	for (auto i : def1) {
		for (auto j : def2) {
			if (i.second + j.second <= 6) {
				w1.clue[0] = i;
				w2.clue[0] = j;
				sq.lines = i.second + j.second;
				sq.clues = { i.first, j.first };
				return;
			}
		}
	}

	int optionBr = 1;
	cout << "actualwords :" << w1Actual << " / " << w2Actual << endl;
	for (auto i : alternatives[w1Actual]) {
		for (auto j : alternatives[w2Actual]) {
			if (i.second + j.second <= 6) {
				if (!shown)cout << "optionstart: " << endl <<
					to_upper(w1.clue[0].first) << endl << " " << endl << to_upper(w2.clue[0].first) << endl << "!!!" << endl;
				shown = 1;
				cout << i.first << endl << " " << endl << j.first << endl << "!!!" << endl;
				options.push_back({ i,j });
			}
		}
	}
	cout << "optionend" << endl;
	if (options.empty())return;
	int userInput;
	cin >> userInput;
	userInput -= 1;
	cout << "waitforme" << endl;
	if (userInput <= 0 || userInput > options.size())return;
	w1.clue[0] = options[userInput - 1].first;
	w2.clue[0] = options[userInput - 1].second;
	sq.lines = options[userInput - 1].first.second + options[userInput - 1].second.second;
	sq.clues = { options[userInput - 1].first.first, options[userInput - 1].second.first };
}

vector<string> Smith::split(const string& str, const string& delim)
{
	vector<string> tokens;
	size_t prev = 0, pos = 0;
	do
	{
		pos = str.find(delim, prev);
		if (pos == string::npos) pos = str.length();
		string token = str.substr(prev, pos - prev);
		if (!token.empty()) tokens.push_back(token);
		prev = pos + delim.length();
	} while (pos < str.length() && prev < str.length());
	return tokens;
}

void Smith::parseDictAlternative()
{
	if (!alternatives.count("dict"))return;

	for (auto &i : alternatives["dict"]) {
		auto pos = i.first.find(":");
		string allwords = i.first.substr((pos==string::npos?0:pos+2));
		vector<string> tokens = split(allwords, ", ");
		forHelpDict.insert(tokens.begin(), tokens.end());
	}

	string altString = "ÐÅ×ÍÈÊ: ";
	for (auto &i : forHelpDict) {
		altString += i;
		altString += ", ";
	}
	if (!forHelpDict.empty()) {
		altString.pop_back();
		altString.pop_back();
	}
	alternatives["dict"] = map<string, int>{ {altString,1} };
}

void Smith::loadAlternativeClues(duma& w)
{
	string wActual = w.actualWord;
	vector<pair<pair<string, int>, pair<string, int>>> options;

	//find if there is an alternative of the chosen explanation

	for (auto i : alternatives[wActual]) {
		int min1 = 10000000, ind(-1);

		for (int j = 0; j < w.allClues.size(); j++) {
			int dist = dictionary::levenstein(i.first, to_upper(w.allClues[j].first));
			if (dist < min1) {
				ind = j;
				min1 = dist;
			}
		}

		if (ind == w.clueInd && min1>0) {
			bool hasOne = false;
			for (int j = 0; j < w.clue.size(); j++)
				if (w.clue[j].second == i.second) {
					hasOne = true;
					w.clue[j] = i; // Always put the alternative one over the generated one.
				}
			if(!hasOne)w.clue.push_back(i);
		}
	}
}

string Smith::to_upper(string s)
{
	for (int i = 0; i < s.size(); i++) {
		uc c = s[i];
		if (c >= crossword::cyrillicA && c <= crossword::cyrillicA + 31) {
			c -= 32;
		}
		s[i] = c;
	}
	return s;
}

string Smith::clearString(string s)
{
	string clean;
	for (int i = 0; i < s.size(); i++) {
		if (uc(s[i]) > crossword::cyrillicA - 33 && uc(s[i]) < crossword::cyrillicA + 32)clean.push_back(s[i]);
	}
	return clean;
}

vector<string> Smith::splitExplanation(string explanation)
{
	vector<string> clues;
	size_t prev = 0;

	size_t pos = 0;
	do {
		pos = explanation.find(" !!! ", prev);
		if (pos != string::npos) {
			clues.push_back(explanation.substr(prev, pos - prev));
			prev = pos + 5;
		}
	} while (pos != string::npos);
	clues.push_back(explanation.substr(prev));
	for (int i = 0; i < clues.size(); i++) {
		pos = clues[i].find('/');
		if (pos != string::npos) {
			size_t pos2 = clues[i].find('/', pos + 1);
			if (pos2 != string::npos) {
				clues[i] = clues[i].substr(0, pos) + clues[i].substr(pos2 + 1);
			}
		}
	}
	return clues;
}

void Smith::normalizeFigures() {
	for (int i = 0; i < boxes.size(); i++)
	{
		double minX = 1000000, minY = 10000000, maxY = -1000000, maxX = -1000000, currSide;
		for (int j = 0; j < boxes[i].points.size(); j++) {
			auto &p = boxes[i].points[j];
			minX = min(minX, p.first);
			minY = min(minY, p.second);
			maxY = max(maxY, p.second);
			maxX = max(maxX, p.first);
		}
		currSide = max(maxY - minY, maxX - minX);
		for (int j = 0; j < boxes[i].points.size(); j++) {
			boxes[i].points[j].first -= minX;
			boxes[i].points[j].second -= minY;
		}
		if (defaultSide != 0) {
			double coef = defaultSide / currSide;
			for (int j = 0; j < boxes[i].points.size(); j++)
			{
				boxes[i].points[j].first *= coef;
				boxes[i].points[j].second *= coef;
			}
		}
		else {
			defaultSide = currSide;
		}
	}
	for (int i = 0; i < arrows.size(); i++)
	{
		double minX = 10000000, minY = 10000000, maxY = -10000000, maxX = -10000000, currSide;
		for (int j = 0; j < arrows[i].points.size(); j++) {
			auto &p = arrows[i].points[j];
			minX = min(minX, p.first);
			minY = min(minY, p.second);
			maxY = max(maxY, p.second);
			maxX = max(maxX, p.first);
		}
		for (int j = 0; j < arrows[i].points.size(); j++)
		{
			arrows[i].points[j].first -= minX;
			arrows[i].points[j].second -= minY;
		}
		currSide = max(maxY - minY, maxX - minX);
		double coef = (defaultSide * arrowShrinkRay) / currSide;
		for (int j = 0; j < arrows[i].points.size(); j++)
		{
			arrows[i].points[j].first  *= coef;
			arrows[i].points[j].second *= coef;
		}
	}
}

void Smith::placeClues(int m, int br)
{
	static int last_update = -1;
	if (last_update != -1)last_update++;
	if (br > bestBr)return;
	if (m >= words.size()) {
		int brSwap = 0;
		for (int i = 0; i < cross.N; i++) {
			for (int j = 0; j < cross.M; j++) {
				for (int k = 0; k < squares[i][j].word_clue_ind.size(); k++) {
					if (words[squares[i][j].word_clue_ind[k].first].defaultSquare != make_pair(i, j))brSwap++;
				}
			}
		}
		if (br < bestBr) {
			cout << "found" << endl;
			bestBr = br;
			bestSwap = brSwap;
			bestSol = squares;
			last_update = 0;
		}
		else if (br == bestBr) {
			if (brSwap < bestSwap) {
				cout << "found" << endl;
				bestBr = br;
				bestSwap = brSwap;
				bestSol = squares;
				last_update = 0;
			}
		}
		return;
	}
	duma &w = words[m];
	if (last_update != -1 && rand() % 15 == 1)return;
	if (last_update > 10000000)return;
	if (w.clue.empty() || w.adjacent.empty()) {
		placeClues(m + 1, br);
		return;
	}
	for (int i = 0; i < w.clue.size(); i++) {
		for (int j = 0; j < w.adjacent.size(); j++) {
			squares[w.adjacent[j].first][w.adjacent[j].second].clues.push_back(w.clue[i].first);
			squares[w.adjacent[j].first][w.adjacent[j].second].word_clue_ind.push_back(make_pair(m, i));
			squares[w.adjacent[j].first][w.adjacent[j].second].lines += w.clue[i].second;
			squares[w.adjacent[j].first][w.adjacent[j].second].has_anomaly += w.anomaly;

			if (squares[w.adjacent[j].first][w.adjacent[j].second].clues.size() < 3) {
				placeClues(m + 1, br + (squares[w.adjacent[j].first][w.adjacent[j].second].lines > 6));
			}
			if (w.anomaly || squares[w.adjacent[j].first][w.adjacent[j].second].has_anomaly) {
				placeClues(m + 1, br + (squares[w.adjacent[j].first][w.adjacent[j].second].lines > 6));
			}

			squares[w.adjacent[j].first][w.adjacent[j].second].clues.pop_back();
			squares[w.adjacent[j].first][w.adjacent[j].second].word_clue_ind.pop_back();
			squares[w.adjacent[j].first][w.adjacent[j].second].lines -= w.clue[i].second;
			squares[w.adjacent[j].first][w.adjacent[j].second].has_anomaly -= w.anomaly;
		}
	}
	/*cout << w.clues[0].first << endl;
	cout << w.adjacent[0].first << " " << w.adjacent[0].second << endl;
	cout << squares[w.adjacent[0].first][w.adjacent[0].second].clues.size() << endl;
	cout << m << " " << w.clues.size() << " " << w.adjacent.size() << " " << br << " " << bestBr << endl;
	cout << (w.anomaly ? "Anomaly" : "Normal");
	getchar();*/
}

int Smith::getWordHeightType(int i, int j, duma &w) {
	if (i > w.i)return 1;//top
	if (i < w.i)return 2;//bot
	return 3;
}

void Smith::placeClues_Clusters(int m, double br, int n)
{
	if (finishSearch && bestBr < 1000) {
		cout << "returning because time is up!" << endl;
		return;
	}
	if (br >= bestBr)return;
	if (m >= clusters[n].size()) {
		if (br < bestBr) {
			cout << "found better" << endl;
			bestBr = br;
			bestSol = squares;
		}
		return;
	}
	duma &w = words[clusters[n][m]];
	if (w.clue.empty() || w.adjacent.empty()) {
		placeClues_Clusters(m + 1, br, n);
		return;
	}

	for (int i = 0; i < w.clue.size(); i++) {
		for (int j = 0; j < w.adjacent.size(); j++) {
			squares[w.adjacent[j].first][w.adjacent[j].second].clues.push_back(w.clue[i].first);
			squares[w.adjacent[j].first][w.adjacent[j].second].word_clue_ind.push_back(make_pair(clusters[n][m], i));
			squares[w.adjacent[j].first][w.adjacent[j].second].lines += w.clue[i].second;
			int wordHType = getWordHeightType(w.adjacent[j].first, w.adjacent[j].second, w);

			if (wordHType == 1) {
				if (squares[w.adjacent[j].first][w.adjacent[j].second].ForbidTop) goto skipThis;
				squares[w.adjacent[j].first][w.adjacent[j].second].ForbidTop = true;
			}
			if (wordHType == 2) {
				if (squares[w.adjacent[j].first][w.adjacent[j].second].ForbidBot) goto skipThis;
				squares[w.adjacent[j].first][w.adjacent[j].second].ForbidBot = true;
			}

			if (squares[w.adjacent[j].first][w.adjacent[j].second].clues.size() < 4) {
				double drawback = 0;
				if (squares[w.adjacent[j].first][w.adjacent[j].second].lines >= 8) drawback += 1.0;
				if (squares[w.adjacent[j].first][w.adjacent[j].second].lines >= 8 && squares[w.adjacent[j].first][w.adjacent[j].second].clues.size()>=3) drawback += 1.0;
				if (squares[w.adjacent[j].first][w.adjacent[j].second].lines == 7) drawback += 0.50;
				if (w.defaultSquare != w.adjacent[j]) drawback += 0.25;
				placeClues_Clusters(m + 1, br + drawback, n);
			}

			if (wordHType == 1) {
				squares[w.adjacent[j].first][w.adjacent[j].second].ForbidTop = false;
			}
			if (wordHType == 2) {
				squares[w.adjacent[j].first][w.adjacent[j].second].ForbidBot = false;
			}
		skipThis:;
			squares[w.adjacent[j].first][w.adjacent[j].second].clues.pop_back();
			squares[w.adjacent[j].first][w.adjacent[j].second].word_clue_ind.pop_back();
			squares[w.adjacent[j].first][w.adjacent[j].second].lines -= w.clue[i].second;
		}
	}

}

void Smith::createCluster(int m, vector<bool>& used, vector<int> &cluster){
	if (used[m])return;
	used[m] = 1;
	cluster.push_back(m);
	for (int i = 0; i < words.size(); i++) {
		if (used[i])continue;
		for (int j = 0; j < words[m].adjacent.size(); j++) {
			for (int k = 0; k < words[i].adjacent.size(); k++) {
				if (words[m].adjacent[j] == words[i].adjacent[k]) {
					createCluster(i, used, cluster);
					break;
				}
			}
		}
	}
}

double Smith::calculateSD(vector<double> data)
{
	double ave = 0, ave1 = 0;
	for (int i = 0; i < data.size(); i++)ave += data[i];
	ave /= data.size();
	for (int i = 0; i < data.size(); i++)ave1 += fabs(data[i] - ave);
	return ave1 / data.size();
}

vector<Fig> Smith::loadFigures(string fileName) {
	ifstream fin(fileName);
	vector<Fig> figs;
	if (!fin.good()) {
		std::cout << "Could not open " << fileName << endl;
		exit(0);
	}
	while (fin.good()) {
		int numPoints;
		fin >> numPoints;
		figs.push_back(Fig());
		figs.back().points.reserve(numPoints);
		for (int i = 0; i < numPoints; i++) {
			double x, y;
			fin >> x >> y;
			figs.back().points.push_back(make_pair(x, y));
		}
	}
	return figs;
}

pair<string, int> Smith::cutClue(string clue)
{
	auto tokens = tokenize(clue);
	vector<set<int>> separated;
	for (int i = 0; i < int(tokens.size()); i++)
		separated.push_back(separate(tokens[i]));
	
	string bestClue = clue;
	int bestCuts = 100000;
	int bestLines = 100000;
	double bestSigma = 10000;

	recSplit(tokens,separated,"",1,0,0,bestClue,bestLines,bestCuts,bestSigma);

	while (std::isspace(bestClue.back())) bestClue.pop_back();
	bestLines = 1;
	for (int i = 0; i < bestClue.size(); i++)bestLines += (bestClue[i] == '\n');

	return make_pair(bestClue, bestLines);
}

set<int> Smith::separate(string word) {
	if (word.empty() || word.size() < 4)return std::set<int>();
	bool isDate = false;
	std::vector<int> hyphenInd;
	for (int i = 0; i < int(word.size()); i++) {
		if (std::isdigit(word[i])) {
			isDate = true;
			break;
		}
		if (word[i] == '-' || word[i] == '.') {
			if (i != 0 && i != int(word.size()) - 1)hyphenInd.push_back(i);
		}
	}
	if (!isDate && !hyphenInd.empty()) {
		std::set<int> res;
		int prevIndex = 0;
		for (int i = 0; i < int(hyphenInd.size()); i++) {
			auto res_i = separate(word.substr(prevIndex, hyphenInd[i] - prevIndex));
			for (auto j : res_i)res.insert(j + prevIndex);
			res.insert(hyphenInd[i]);
			prevIndex = hyphenInd[i] + 1;
		}
		auto res_last = separate(word.substr(prevIndex, int(word.size()) - prevIndex));
		for (auto j : res_last)res.insert(j + prevIndex);
		return res;
	}

	static std::set<char> consonants{ 'é','á','â','ã','ä','æ','ç','ê','ë','ì','í','ï','ð','ñ','ò','ô','õ','ö','÷','ø','ù',
									  'É','Á','Â','Ã','Ä','Æ','Ç','Ê','Ë','Ì','Í','Ï','Ð','Ñ','Ò','Ô','Õ','Ö','×','Ø','Ù' };
	static std::set<char> vowels{ 'à','ú','å','è','î','ó','þ','ÿ','À','Ú','Å','È','Î','Ó','Þ','ß' };

	bool passedAVowel = false;
	bool passedADigit = false;

	int consCounter = 0;
	int vowCounter = 0;
	int firstVowel = word.size() - 1, firstTwoLetters = 2, lastTwoLetters = word.size() - 3;

	std::vector<int> breakIndice;

	for (int i = 0, letterBr = 0; i < int(word.size()); i++) {
		if (vowels.count(word[i]) && firstVowel == (int(word.size()) - 1)) {
			firstVowel = i;
		}
		if (uc(word[i]) >= crossword::cyrillicA - 32 && uc(word[i]) <= crossword::cyrillicA + 31) { // is letter
			letterBr += 1;
			if (letterBr == 2) {
				firstTwoLetters = i;
				if (firstVowel != (int(word.size()) - 1))break;
			}
		}
	}
	for (int i = int(word.size()) - 1, letterBr = 0; i >= 0; i--) {
		if (uc(word[i]) >= crossword::cyrillicA - 32 && uc(word[i]) <= crossword::cyrillicA + 31) { // is letter
			letterBr += 1;
			if (letterBr == 2) {
				lastTwoLetters = i - 1;
				break;
			}
		}
	}
	for (int i = int(word.size()) - 1; i >= 0; i--) {
		if (vowels.count(word[i])) {
			if (consCounter == 1 && i >= firstTwoLetters && i <= lastTwoLetters) {
				breakIndice.push_back(i);
			}
			passedAVowel = true;
			vowCounter += 1;
			if (vowCounter >= 2 && i >= firstTwoLetters && i <= lastTwoLetters) {
				breakIndice.push_back(i);
			}
			consCounter = 0;
			continue;
		}
		if (passedAVowel && consonants.count(word[i])) {
			consCounter += 1;
			if (consCounter >= 2 && i >= firstTwoLetters && i <= lastTwoLetters && firstVowel < i) {
				breakIndice.push_back(i);
			}
			vowCounter = 0;
		}
		if (std::isdigit(word[i])) {
			passedADigit = true;
		}
		if ((word[i] == '-' || word[i] == '.') && i >= firstTwoLetters && i <= lastTwoLetters) {
			if (passedADigit)continue; // This is probably a period and cannot be divided here.
			breakIndice.push_back(i);
			consCounter = 0;
		}
	}
	return std::set<int>(breakIndice.begin(), breakIndice.end());
}

void Smith::recSplit(std::vector<std::string>& tokens, std::vector<std::set<int>>& separated, std::string currClue,
	int lines, int cuts, double len,
	std::string& bestClue, int& bestLines, int& bestCuts, double& bestSigma, int i, int j) {
	if (lines > bestLines)return;
	else if (lines == bestLines) {
		if (cuts > bestCuts)return;
	}

	static double threshold = 10.0 * fabs(squareSizeCoefX);
	static std::map<char, double> charWidth{
		{' ',0.6},{'(',0.6},{')',0.6},{',',0.6},{'.',0.6},{'1',0.6},{'\'',0.6},{'\"',0.6},{'-',0.6},
		{'Þ',1.4},{'þ',1.4},{'Ø',1.4},{'ø',1.4},{'Ù',1.4},{'ù',1.4},{'Æ',1.4},{'æ',1.4},{'Ì',1.4},{'ì',1.4},
		{'Ô',1.1},{'ô',1.1}
	};
	for (; i < int(tokens.size()); i++) {
		auto sepPoints = separated[i];
		while (j < int(tokens[i].size())) {
			len += (charWidth.count(tokens[i][j]) ? charWidth[tokens[i][j]] : 0.95);
			if (len > threshold)return;
			currClue.push_back(tokens[i][j]);

			if (sepPoints.count(j)) {
				bool put = false;
				if (currClue.back() != '-')currClue.push_back('-'), put = true;
				if (len + 0.5 <= threshold) {
					currClue.push_back('\n');
					recSplit(tokens, separated, currClue, lines + 1, cuts + 1, 0, bestClue, bestLines, bestCuts, bestSigma, i, j + 1);
					currClue.pop_back();
				}
				if (put)currClue.pop_back();
			}
			j++;
		}
		if (i != int(tokens.size()) - 1) {
			currClue.push_back('\n');

			recSplit(tokens, separated, currClue, lines + 1, cuts, 0, bestClue, bestLines, bestCuts, bestSigma,
				i + (tokens[i + 1][0] == ' ' ? 2 : 1), 0);

			currClue.pop_back();
		}
		j = 0;
	}
	std::vector<double> data(1, 0.0);
	for (int i = 0; i < currClue.size(); i++) {
		if (currClue[i] == '\n')data.push_back(0), i++;
		data.back() += (charWidth.count(currClue[i]) ? charWidth[currClue[i]] : 0.95);
	}
	double sd = calculateSD(data);
	if (lines < bestLines) {
		bestClue = currClue;
		bestLines = lines;
		bestCuts = cuts;
		bestSigma = sd;
	}
	else if (lines == bestLines) {
		if (cuts < bestCuts) {
			bestClue = currClue;
			bestLines = lines;
			bestCuts = cuts;
			bestSigma = sd;
		}
		else if (cuts == bestCuts) {
			if (sd < bestSigma) {
				bestClue = currClue;
				bestLines = lines;
				bestCuts = cuts;
				bestSigma = sd;
			}
		}
	}
}

bool Smith::isValidLetter(uc c)
{
	return ((c >= crossword::cyrillicA - 32 && c <= crossword::cyrillicA + 31) || (c >= '0' && c <= '9') || (c == '(' || c == ')' || c == '\"' || c == ',' || c == '.' || c == '-'));
}

vector<string> Smith::tokenize(string s)
{
	vector<string> tokens(1);
	for (int i = 0; i < s.size(); i++) {
		if (!isValidLetter(s[i])) {
			tokens.push_back(string());
			tokens.back().push_back(s[i]);
			tokens.push_back(string());
		}
		else {
			tokens.back().push_back(s[i]);
		}
	}
	return tokens;
}

void Smith::setCrossword(crossword cross) {
	this->cross = cross;
	prepare();
}

void Smith::replace(string & str, string from, string to){
	if (from.empty())
		return;
	size_t start_pos = 0;
	while ((start_pos = str.find(from, start_pos)) != std::string::npos) {
		str.replace(start_pos, from.length(), to);
		start_pos += to.length(); // In case 'to' contains 'from', like replacing 'x' with 'yx'
	}
}