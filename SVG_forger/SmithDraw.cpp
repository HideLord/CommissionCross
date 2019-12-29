#include "Smith.h"
#include <sstream>

SVG::SVG_element Smith::ClueToSVG(string clue, string anchor, double x, double y, string color, string actualWord)
{
	SVG::SVG_element t("text");
	t["id"] = actualWord;
	t.addAttribute("font-size", "2.15788889");
	t.addAttribute("font-family", "HebarCond");
	t.addAttribute("font-weight", "bold");
	t.addAttribute("line-height", "0.93");
	t.addAttribute("fill", color);
	t.addAttribute("x", to_string(x));
	t.addAttribute("y", to_string(y));
	t.addAttribute("xml:space", "preserve");
	
	string currentLine;
	for (int i = 0; i < clue.size(); i++) {
		if (clue[i] == '\n' && !currentLine.empty()) {
			SVG::SVG_element tspan("tspan");
			tspan.setContent(currentLine);
			tspan["text-anchor"] = anchor;
			tspan["sodipodi:role"] = "line";
			tspan["x"] = to_string(x);
			tspan["xml:space"] = "preserve";
			t.add_copy(tspan);
			currentLine.clear();
		}
		else currentLine.push_back(clue[i]);
	}
	if (!currentLine.empty()) {
		SVG::SVG_element tspan("tspan");
		tspan.setContent(currentLine);
		tspan["text-anchor"] = anchor;
		tspan["sodipodi:role"] = "line";
		tspan["x"] = to_string(x);
		tspan["xml:space"] = "preserve";
		t.add_copy(tspan);
	}
	return t;
}

Smith::Smith()
{
}

Smith::Smith(crossword cross)
{
	setCrossword(cross);
}

Smith::~Smith()
{
}

void Smith::drawCrossword()
{
	using namespace SVG;
	string defaultTextColor = "black";
	if (squareFill == "#FF000000") {
		defaultTextColor = "white";
	}

	{
		auto temp = decodeColor(squareFill);
		squareFill = temp.first;
		squareOp = temp.second;
		temp = decodeColor(setFill);
		setFill = temp.first;
		setOp = temp.second;
		temp = decodeColor(arrowFill);
		arrowFill = temp.first;
		arrowOp = temp.second;
	}
	

	auto root = GetRoot(14*cross.M + 42 + 8 * cross.M + 7, 14*cross.N + 28 + 14);
	
	Fig currSet = boxes[setInd];
	Fig currSquare = boxes[squareInd];
	Fig currArrow = arrows[arrowInd];

	auto boxGroup = SVG::SVG_element("g", { {"id","boxes"}, {"inkscape:label","squares"}, {"inkscape:groupmode","layer"} });
	auto netGroup = SVG::SVG_element("g", { {"id","net"}, {"inkscape:label","set"}, {"inkscape:groupmode","layer"} });
	auto arrowGroup = SVG::SVG_element("g", { {"id","arrows"}, {"inkscape:label","arrows"}, {"inkscape:groupmode","layer"} });
	auto textGroup = SVG::SVG_element("g", { {"id","text"}, {"inkscape:label","text"}, {"inkscape:groupmode","layer"} });

	double fromTop = 1.35, fromSide = 0.85;

	double coef = 14.0 / currSet.getWidth();
	currSet.scale(coef, coef);
	currSet.scaleUniform(setSizeCoefX, setSizeCoefY);
	currSet.rotate(setRotation, currSet.getCenter());
	double setSideW = currSet.getWidth();
	double setSideH = currSet.getHeight();
	
	for (int i = 0; i < cross.N; i++) {
		for (int j = 0; j < cross.M; j++) {
			if (cross.isSpecialBox(i, j))continue;
			SVG_element setBox("path");
			currSet.translate(j * 14, i * 14);
			setBox["d"] = currSet.getSvgPath();
			setBox["stroke"] = "black";
			setBox["stroke-width"] = to_string(0.2*coef);
			setBox["fill"] = setFill;
			setBox["fill-opacity"] = to_string(setOp);
			currSet.translate(-j * 14, -i * 14);
			netGroup.add_copy(setBox);
		}
	}

	coef = 14.0 / currSquare.getWidth();
	currSquare.scale(coef, coef);
	currSquare.scaleUniform(squareSizeCoefX, squareSizeCoefY);
	currSquare.rotate(squareRotation, currSquare.getCenter());
	double squareSideW = currSquare.getWidth();
	double squareSideH = currSquare.getHeight();

	for (int i = 0; i < cross.N; i++) {
		for (int j = 0; j < cross.M; j++) {
			if (cross.isNormalBox(i, j)) {
				SVG_element squareBox("path");
				currSquare.translate(j * 14, i * 14);
				squareBox["d"] = currSquare.getSvgPath();
				squareBox["stroke"] = "black";
				squareBox["stroke-width"] = to_string(0.5*coef);
				squareBox["fill"] = squareFill;
				squareBox["fill-opacity"] = to_string(squareOp);
				currSquare.translate(-j * 14, -i * 14);
				boxGroup.add_copy(squareBox);
			}
		}
	}

	if (defaultTextColor == "white") {
		fromTop -= (coef*0.25);
		fromSide -= (coef*0.25);
	}

	coef = (14.0*arrowShrinkRay) / max(currArrow.getHeight(), currArrow.getWidth());
	currArrow.scale(coef, coef);
	currArrow.scaleUniform(arrowSizeCoefX, arrowSizeCoefY);
	currArrow.normalize();
	double arrowW = currArrow.getWidth();
	double arrowH = currArrow.getHeight();

	double delta = 1, squareOutW = (squareSideW - 14) / 2, squareOutH = (squareSideH - 14) / 2;

	auto currArrow90 = currArrow;
	currArrow90.rotate(90, currArrow90.getCenter());
	currArrow90.normalize();
	double arrow90W = currArrow90.getWidth();
	double arrow90H = currArrow90.getHeight();

	double letterHTop = 1.5;
	double letterH = 1.95;

	for (int i = 0; i < cross.N; i++) {
		for (int j = 0; j < cross.M; j++) {
			if (!cross.isNormalBox(i, j)) continue;
			auto & sq = squares[i][j];
			for (int c = 0; c < sq.clues.size(); c++) {
				auto currWord = words[sq.word_clue_ind[c].first];
				if (i > currWord.i && j == currWord.j) {
					if (currWord.hor) { // top down 0
						auto arrowPath = SVG_element("path");
						currArrow.translate(j * 14 - squareOutW + squareSideW / 6, i * 14 - squareOutH - delta - arrowH);
						arrowPath["d"] = currArrow.getSvgPath();
						arrowPath["stroke-width"] = "0.04";
						arrowPath["stroke"] = "black";
						arrowPath["fill"] = arrowFill;
						arrowPath["fill-opacity"] = to_string(arrowOp);
						currArrow.translate(-(j * 14 - squareOutW + squareSideW / 6), -(i * 14 - squareOutH - delta - arrowH));
						arrowGroup.add_copy(arrowPath);
						string color = "purple";
						textGroup.add_copy(ClueToSVG(sq.clues[c], "start", j * 14 - squareOutW + fromSide, i * 14 - squareOutH + fromTop + letterHTop, color,currWord.actualWord));
						sq.topOccupied = 1;
					}
					else { // Error
						auto arrowPath = SVG_element("path");
						currArrow90.translate((j + 0.5) * 14 - arrow90W/2, i * 14 - squareOutH - delta - arrowH);
						arrowPath["d"] = currArrow90.getSvgPath();
						arrowPath["stroke-width"] = "0.04";
						arrowPath["stroke"] = "black";
						arrowPath["fill"] = arrowFill;
						arrowPath["fill-opacity"] = to_string(arrowOp);
						currArrow90.translate(-((j + 0.5) * 14 - arrow90W / 2), -(i * 14 - squareOutH - delta - arrowH));
						arrowGroup.add_copy(arrowPath);
						string color = "purple";
						textGroup.add_copy(ClueToSVG(sq.clues[c], "middle", (j+0.5)*14 - squareOutW + fromSide, i * 14 - squareOutH + fromTop + letterHTop, color, currWord.actualWord));
						sq.topOccupied = 1;
					}
				}
			}
			for (int c = 0; c < sq.clues.size(); c++) {
				auto currWord = words[sq.word_clue_ind[c].first];
				if (j < currWord.j) {
					if (currWord.hor) { // right up 0
						auto arrowPath = SVG_element("path");
						if (!sq.topOccupied)currArrow.translate((j + 1) * 14 + squareOutW + delta, i * 14 - squareOutH + squareSideH / 4.5);
						else currArrow.translate((j + 1) * 14 + squareOutW + delta, (i+1) * 14 + squareOutH - arrowH - squareSideH / 6);
						arrowPath["d"] = currArrow.getSvgPath();
						arrowPath["stroke-width"] = "0.04";
						arrowPath["stroke"] = "black";
						arrowPath["fill"] = arrowFill;
						arrowPath["fill-opacity"] = to_string(arrowOp);

						if (!sq.topOccupied)currArrow.translate(-((j + 1) * 14 + squareOutW + delta),-(i * 14 - squareOutH + squareSideH / 4.5));
						else currArrow.translate(-((j + 1) * 14 + squareOutW + delta), -((i + 1) * 14 + squareOutH - arrowH - squareSideH / 6));
						arrowGroup.add_copy(arrowPath);
						int allowedSpace = 6 - sq.lines;
						string color;
						if (sq.lines <= 6)color = defaultTextColor;
						if (sq.lines == 7)color = "green";
						if (sq.lines > 7)color = "red";
						if (!sq.topOccupied)textGroup.add_copy(ClueToSVG(sq.clues[c], "end", (j + 1) * 14 + squareOutW - fromSide, i * 14 - squareOutH + fromTop + letterHTop + (allowedSpace > 0 ? letterH / 2 : 0), color, currWord.actualWord));
						else textGroup.add_copy(ClueToSVG(sq.clues[c], "end", (j + 1) * 14 + squareOutW - fromSide, (i+1) * 14 + squareOutH - fromTop - letterH * (currWord.clue[sq.word_clue_ind[c].second].second - 1), color, currWord.actualWord));
						sq.topOccupied = 1;
					}
					else { // right up 90
						auto arrowPath = SVG_element("path");
						if (!sq.topOccupied)currArrow90.translate((j + 1) * 14 + squareOutW + delta, i * 14 - squareOutH + squareSideH / 6);
						else currArrow90.translate((j + 1) * 14 + squareOutW + delta, (i + 1) * 14 + squareOutH - arrowH - squareSideH / 6);
						arrowPath["d"] = currArrow90.getSvgPath();
						arrowPath["stroke-width"] = "0.04";
						arrowPath["stroke"] = "black";
						arrowPath["fill"] = arrowFill;
						arrowPath["fill-opacity"] = to_string(arrowOp);
						if (!sq.topOccupied)currArrow90.translate(-((j + 1) * 14 + squareOutW + delta), -(i * 14 - squareOutH + squareSideH / 6));
						else currArrow90.translate(-((j + 1) * 14 + squareOutW + delta), -((i + 1) * 14 + squareOutH - arrowH - squareSideH / 6));
						arrowGroup.add_copy(arrowPath);
						string color;
						if (sq.lines <= 6)color = defaultTextColor;
						if (sq.lines == 7)color = "green";
						if (sq.lines > 7)color = "red";
						if (i != currWord.i)color = "purple";
						if (!sq.topOccupied)textGroup.add_copy(ClueToSVG(sq.clues[c], "end", (j + 1) * 14 + squareOutW - fromSide, i * 14 - squareOutH + fromTop + letterHTop, color, currWord.actualWord));
						else textGroup.add_copy(ClueToSVG(sq.clues[c], "end", (j + 1) * 14 + squareOutW - fromSide, (i+1) * 14 + squareOutH - fromTop - letterH * (currWord.clue[sq.word_clue_ind[c].second].second - 1), color, currWord.actualWord));
						sq.topOccupied = 1;
					}
				}
				
				if (i < currWord.i) {
					if (currWord.hor) { // bot up 0
						auto arrowPath = SVG_element("path");
						if(currWord.j==0 || cross.isSpecialBox(currWord.i, currWord.j - 1))currArrow.translate(j * 14 - squareOutW + squareSideW / 6, (i+1) * 14 + squareOutH + delta);
						else currArrow.translate((j+0.5) * 14, (i + 1) * 14 + squareOutH + delta);
						arrowPath["d"] = currArrow.getSvgPath();
						arrowPath["stroke-width"] = "0.04";
						arrowPath["stroke"] = "black";
						arrowPath["fill"] = arrowFill;
						arrowPath["fill-opacity"] = to_string(arrowOp);
						if (currWord.j == 0 || cross.isSpecialBox(currWord.i, currWord.j - 1))currArrow.translate(-(j * 14 - squareOutW + squareSideW / 6), -((i + 1) * 14 + squareOutH + delta));
						else currArrow.translate(-((j + 0.5) * 14), -((i + 1) * 14 + squareOutH + delta));
						arrowGroup.add_copy(arrowPath);
						string color;
						if (sq.lines <= 6)color = defaultTextColor;
						if (sq.lines == 7)color = "green";
						if (sq.lines > 7)color = "red";
						if (currWord.j == 0 || cross.isSpecialBox(currWord.i, currWord.j - 1))textGroup.add_copy(ClueToSVG(sq.clues[c], "start", j * 14 - squareOutW + fromSide, (i + 1) * 14 + squareOutH - fromTop - letterH * (currWord.clue[sq.word_clue_ind[c].second].second-1),color, currWord.actualWord));
						else textGroup.add_copy(ClueToSVG(sq.clues[c], "middle", (j + 0.5) * 14, (i + 1) * 14 + squareOutH - fromTop - letterH * (currWord.clue[sq.word_clue_ind[c].second].second - 1), color, currWord.actualWord));
						sq.botOccupied = 1;
					}
					else { // bot up 90 (centered)
						auto arrowPath = SVG_element("path");
						currArrow90.translate((j+0.5) * 14 - arrow90W / 2, (i+1) * 14 + squareOutH + delta);
						arrowPath["d"] = currArrow90.getSvgPath();
						arrowPath["stroke-width"] = "0.04";
						arrowPath["stroke"] = "black";
						arrowPath["fill"] = arrowFill;
						arrowPath["fill-opacity"] = to_string(arrowOp);
						currArrow90.translate(-((j + 0.5) * 14 - arrow90W/2), -((i + 1) * 14 + squareOutH + delta));
						arrowGroup.add_copy(arrowPath);
						string color;
						if (sq.lines <= 6)color = defaultTextColor;
						if (sq.lines == 7)color = "green";
						if (sq.lines > 7)color = "red";
						textGroup.add_copy(ClueToSVG(sq.clues[c], "middle", (j+0.5) * 14, (i + 1) * 14 + squareOutH - fromTop - letterH * (currWord.clue[sq.word_clue_ind[c].second].second - 1),color, currWord.actualWord));
						sq.botOccupied = 1;
					}
				}
			}
			for (int c = 0; c < sq.clues.size(); c++) {
				auto currWord = words[sq.word_clue_ind[c].first];
				if (j > currWord.j && i == currWord.i) {
					if (currWord.hor) { // Error
						
						auto arrowPath = SVG_element("path");
						if (!sq.topOccupied)currArrow.translate(j * 14 - squareOutW - delta - arrow90W, i * 14 - squareOutH + squareSideH / 6);
						else currArrow.translate(j * 14 - squareOutW - delta - arrow90W, (i + 1) * 14 + squareOutH - squareSideH / 6 - arrowH);
						arrowPath["d"] = currArrow.getSvgPath();
						arrowPath["stroke-width"] = "0.04";
						arrowPath["stroke"] = "black";
						arrowPath["fill"] = arrowFill;
						arrowPath["fill-opacity"] = to_string(arrowOp);
						if (!sq.topOccupied)currArrow.translate(-(j * 14 - squareOutW - delta - arrow90W), -(i * 14 - squareOutH + squareSideH / 6));
						else currArrow.translate(-(j * 14 - squareOutW - delta - arrow90W), -((i + 1) * 14 + squareOutH - squareSideH / 6 - arrowH));
						arrowGroup.add_copy(arrowPath);
						string color = "purple";

						if (!sq.topOccupied)textGroup.add_copy(ClueToSVG(sq.clues[c], "start", j * 14 - squareOutW + fromSide, i * 14 - squareOutH + fromTop + letterHTop, color, currWord.actualWord));
						else textGroup.add_copy(ClueToSVG(sq.clues[c], "start", j * 14 - squareOutW + fromSide, (i + 1) * 14 + squareOutH - fromTop - letterH * (currWord.clue[sq.word_clue_ind[c].second].second - 1), color, currWord.actualWord));
					}
					else { // left up 90
						auto arrowPath = SVG_element("path");
						if (!sq.topOccupied)currArrow90.translate(j * 14 - squareOutW - delta - arrow90W, i * 14 - squareOutH + squareSideH / 6);
						else currArrow90.translate(j * 14 - squareOutW - delta - arrow90W, (i + 1) * 14 + squareOutH - squareSideH / 6 - arrow90H);
						arrowPath["d"] = currArrow90.getSvgPath();
						arrowPath["stroke-width"] = "0.04";
						arrowPath["stroke"] = "black";
						arrowPath["fill"] = arrowFill;
						arrowPath["fill-opacity"] = to_string(arrowOp);
						if (!sq.topOccupied)currArrow90.translate(-(j * 14 - squareOutW - delta - arrow90W), -(i * 14 - squareOutH + squareSideH / 6));
						else currArrow90.translate(-(j * 14 - squareOutW - delta - arrow90W), -((i + 1) * 14 + squareOutH - squareSideH / 6 - arrow90H));
						arrowGroup.add_copy(arrowPath);
						string color = "purple";
						if (!sq.topOccupied)textGroup.add_copy(ClueToSVG(sq.clues[c], "start", j * 14 - squareOutW + fromSide, i * 14 - squareOutH + fromTop + letterHTop, color, currWord.actualWord));
						else textGroup.add_copy(ClueToSVG(sq.clues[c], "start", j * 14 - squareOutW + fromSide, (i + 1) * 14 + squareOutH - fromTop - letterH * (currWord.clue[sq.word_clue_ind[c].second].second - 1), color, currWord.actualWord));
					}
				}
			}
		}
	}

	string author, dictColor = "black";
	string crossFileName = cross.name;
	const size_t last_slash_idx = crossFileName.find_last_of("\\/");
	if (std::string::npos != last_slash_idx){
		crossFileName.erase(0, last_slash_idx + 1);
	}

	double dicStroke = 0.0;
	auto dictionaryRect = SVG::GetBox(0.0 + dicStroke/2.0, 14 * cross.N + dicStroke/2.0, 14 * cross.M - dicStroke / 2, 4, dicStroke);
	dictionaryRect["Fill"] = "Cyan";

	if (crossFileName.find("xmn") != string::npos) {
		author = "Симеон Михайлов";
	}
	else if (crossFileName.find("xsg") != string::npos) {
		author = "Стойчо Николов";
	}
	else if (crossFileName.find("xt") != string::npos) {
		author = "Силвия Миланова";
	}
	else if (crossFileName.find("bgf") != string::npos) {
		author = "Кремена Манолова";
	}
	else if (crossFileName.find("xbig") != string::npos) {
		author = "Стойчо Николов";
	}
	else if (crossFileName.find("hoby") != string::npos) {
		author = "Кремена Манолова";
	}
	else if (crossFileName[0] == 'k' && crossFileName[1] >= '0' && crossFileName[1] <= '9') {
		author = "Христо Стойчев";
		dictColor = "White";
		dictionaryRect["fill"] = "black";
		root.add_copy(dictionaryRect);
	}
	else {
		author = "";
	}
	string helpDict = "РЕЧНИК: ";
	int helpDictBr = 0;
	int wordsToPut = ceil((double)words.size() / 13.0);
	set<string> alphaDict;
	vector<string>putInDictString;
	for (auto &i : words) {
		alphaDict.insert(i.actualWord);
	}
	for (auto &i : forHelpDict) {
		if (alphaDict.count(clearString(i)))putInDictString.push_back(i);
	}
	random_shuffle(begin(putInDictString), end(putInDictString));
	sort(putInDictString.begin(), putInDictString.begin() + min(wordsToPut,size(putInDictString)), [this](string& a, string& b) {return clearString(a) < clearString(b); });
	for (int i = 0; i < min(wordsToPut, size(putInDictString)); i++, helpDictBr++) {
		helpDict += putInDictString[i] + ", ";
	}
	if (helpDictBr)helpDict.pop_back(),helpDict.pop_back();

	auto dictionaryRectText = ClueToSVG(helpDict, "start", 0 + 1 + dicStroke, 14 * (int)cross.N + 3.1, dictColor, "dict");
	dictionaryRectText["font-size"] = "3.033888888";
	dictionaryRectText["font-family"] = "HebarCondBlack";
	dictionaryRectText["font-weight"] = "Normal";
	auto dictionaryAuthor = ClueToSVG(author, "end", 14 * (int)cross.M - 1 - dicStroke, 14 * (int)cross.N + 3.1, dictColor, "author");
	dictionaryAuthor["font-size"] = "3.033888888";
	dictionaryAuthor["font-family"] = "HebarCondBlack";
	dictionaryAuthor["font-weight"] = "Normal";

	if (!(crossFileName[0] == 'k' && crossFileName[1] >= '0' && crossFileName[1] <= '9')) {

		dictionaryAuthor["y"] = to_string(14 * (int)cross.N + 5.5);
		dictionaryAuthor["x"] = to_string(14 * (int)cross.M);
		dictionaryRectText["y"] = to_string(14 * (int)cross.N + 5.5);
		dictionaryRectText["x"] = to_string(0);
	}

	SVG_element answers("g");
	auto ansBorder = SVG::GetBox(0, 0, (int)cross.M * 7, (int)cross.N * 7, 0.5);
	answers.add_copy(ansBorder);
	for (int i = 0; i < cross.N; i++) {
		for (int j = 0; j < cross.M; j++) {
			if (cross.isNormalBox(i, j)) {
				auto box = GetBox(j * 7, i * 7, 7, 7,0.5);
				box["fill"] = "black";
				answers.add_copy(box);
			}
			else if (!cross.isBox(i,j)) {
				auto box = GetBox(j * 7, i * 7, 7, 7, 0.5);
				box["fill"] = "white";
				answers.add_copy(box);
				auto let = GetText((j + 0.5) * 7, i * 7 + 5.2, string(1, cross.board[i][j]));
				let["font-family"] = "Arial";
				let["font-weight"] = "bold";
				let["font-size"] = "4.7";
				answers.add_copy(let);
			}
		}
	}

	answers["transform"] = "translate(" + to_string(14 * cross.M + 42) + "," + to_string(cross.N*7) + ")";

	root["viewBox"] = "-14 -14 " + to_string(14 * cross.M + 42 + 8*cross.M + 7) + " " + to_string(14 * cross.N + 28 + 14);
	root.add_copy(dictionaryRectText);
	root.add_copy(dictionaryAuthor);
	root.add_copy(netGroup);
	root.add_copy(boxGroup);
	root.add_copy(arrowGroup);
	root.add_copy(textGroup);
	root.add_copy(answers);
	stringstream buffer;
	buffer << root << endl;
	wstring ws = SVG::SVG_element::to_wide(buffer.str());
#ifdef DEBUG
	FILE* outFile = fopen("aab.svg", "w+,ccs=UTF-8");
#else // DEBUG
	FILE* outFile = fopen("Z:\\Aprn\\aab.svg", "w+,ccs=UTF-8");
#endif
	std::fwrite(ws.c_str(), ws.size() * sizeof(wchar_t), 1, outFile);
	std::fclose(outFile);
	//system("inkscape \"Z:\\Aprn\\aab.svg\" -E \"Z:\\Aprn\\aab.eps\" --export-text-to-path --export-ignore-filters --export-ps-level=2");
}

pair<string,double> Smith::decodeColor(string c)
{
	map<char, int> M;
	M['0'] = 0;
	M['1'] = 1;
	M['2'] = 2;
	M['3'] = 3;
	M['4'] = 4;
	M['5'] = 5;
	M['6'] = 6;
	M['7'] = 7;
	M['8'] = 8;
	M['9'] = 9;
	M['A'] = 10;
	M['B'] = 11;
	M['C'] = 12;
	M['D'] = 13;
	M['E'] = 14;
	M['F'] = 15;
	int alpha = M[c[1]] * 16 + M[c[2]];
	int R = M[c[3]] * 16 + M[c[4]];
	int G = M[c[5]] * 16 + M[c[6]];
	int B = M[c[7]] * 16 + M[c[8]];
	return { "rgb(" + to_string(R) + "," + to_string(G) + "," + to_string(B) + ")" , alpha/255.0};
	//return "rgba(" + to_string(R) + "," + to_string(G) + "," + to_string(B) + "," + to_string(alpha / 255.0) + ")";
}

