function translateContentsSubs(contents) {
	console.log("Translating file...");

	let lines=contents.split("\r\n").join('\n').split("\n");
	let output="";
	let events=false;

	for (const line of lines){
		if (events) {
			if (line.startsWith("Dialogue")) {
				let lineCont=line.split(",,");
				if (lineCont.length==2){//console.log(line);
					let linesOfSubs=lineCont[1].split("\\n");
					
					output+=lineCont[0]+",,";
					for (const sl of linesOfSubs){
						let translated=TranslateSimpleText(lineCont[1]);
						if (linesOfSubs[linesOfSubs.length-1]==sl)output+=translated;
						else output+=translated+"\\n";
					
					}
					output+="\n";
					
				} else output+=line+"\n";
			} else output+=line+"\n";
		}else output+=line+"\n";
		
		if (line=="[Events]") events=true;
	}

	return output;
}
  
function TranslateFile() {
	var link = document.getElementById('downloadFile');
    link.style.display = 'none';

	console.log(document.querySelector("#file-input").files[0]);
	if(document.querySelector("#file-input").files.length == 0) {
		alert('Error : No file selected');
		return;
	}

	// file selected by user
	let file = document.querySelector("#file-input").files[0];

	// file name
	let file_name = file.name;

	// file MIME type
	//let file_type = file.type;

	// file size in bytes
	//let file_size = file.size;

	// new FileReader object
	let reader = new FileReader();

	// event fired when file reading finished
	reader.addEventListener('load', function(e) {
	   // contents of the file
	    let text = e.target.result;
		let translated=TranslateSimpleText(text);
		//makeTextFile(file_name,text);

		
		var link = document.getElementById('downloadFile');
		link.setAttribute('download', file_name);
		link.href = makeTextFile(translated);
		link.style.display = 'block';
		console.log(link.href);
	});
	

	// event fired when file reading failed
	reader.addEventListener('error', function() {
	    alert('Error : Failed to read file');
	});

	reader.readAsText(file);
}

function TranslateSubs() {
	var link = document.getElementById('downloadSubs');
    link.style.display = 'none';

	console.log(document.querySelector("#subs-input").files[0]);
	if(document.querySelector("#subs-input").files.length == 0) {
		alert('Error: No subs selected');
		return;
	}

	let file = document.querySelector("#subs-input").files[0];

	// file name
	let file_name = file.name;

	let reader = new FileReader();

	reader.addEventListener('load', function(e) {
	    let text = e.target.result;
		let translated=translateContentsSubs(text);
		
		var link = document.getElementById('downloadSubs');
		link.setAttribute('download', file_name);
		link.href = makeTextFile(translated);
		link.style.display = 'block';
		console.log(link.href);
	});
	
	reader.addEventListener('error', function() {
	    alert('Error : Failed to read subs');
	});

	reader.readAsText(file);
}


var textFile= null;
makeTextFile = function (text) {
	var data = new Blob([text], { type: "text/html" });

	if (textFile !== null) {
		window.URL.revokeObjectURL(textFile);
	}

	textFile = window.URL.createObjectURL(data);

	return textFile;
};
