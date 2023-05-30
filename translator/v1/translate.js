var idPops=0;
let SimplyfiedReplacedRules=[];

class ItemSentence {
	constructor() {
		this.input = "";
		this.output = "";
	}

	static Load(data) {
		let item=new ItemSentence();
        let raw = data.split('|');
        item.input=raw[0];
        item.output=raw[1];
		return item;
    }
	GetDicForm(name) {		
		return [this.input, this.output, "", name];
	}
}

class ItemSentencePart {
	constructor() {
		this.input = "";
		this.output = "";
	}

	static Load(data) {
		let item=new ItemSentencePart();
        let raw = data.split('|');
        item.input=raw[0];
        item.output=raw[1];
		return item;
    }

	
	GetDicForm(name) {		
		return [this.input, this.output, "", name];
	}
}

class ItemPatternNoun {
	constructor() {
		this.Name = "";
		this.Gender = -1;
		this.Shapes = [];
	}

	static Load(data) {
		let raw = data.split('|');
		if (raw.length != 14 + 2) {
			if (dev)console.log("PatternNoun - Chybná délka");
			return null;
		}
		let item = new ItemPatternNoun();
		item.Name = raw[0];
		if (raw[1]=="0") item.Gender = "str";
		if (raw[1]=="1") item.Gender = "zen";
		if (raw[1]=="2") item.Gender = "muz ziv";
		if (raw[1]=="3") item.Gender = "muz neziv";
		
		item.Shapes = [raw[2].split(','), raw[3].split(','), raw[4].split(','), raw[5].split(','), raw[6].split(','), raw[7].split(','), raw[8].split(','), raw[9].split(','), raw[10].split(','), raw[11].split(','), raw[12].split(','), raw[13].split(','), raw[14].split(','), raw[15].split(',')];
		return item;
	}
}

class ItemNoun {
	static pattensFrom;
	static pattensTo;

	constructor() {
		this.From = "";
		this.To = "";
		this.PatternFrom = null;
		this.PatternTo = null;
	}
	
	static Load(data) {
		if (data.includes('?')) return null;
		let raw = data.split('|');
		if (raw.length == 4) {
			let item = new ItemNoun();
			item.From = raw[0];
			item.To = raw[1];

			let paternFrom = this.GetPatternByNameFrom(raw[2]);
			if (paternFrom == null) return null;
			else item.PatternFrom = paternFrom;

			let paternTo = this.GetPatternByNameTo(raw[3]);
			if (paternTo == null) return null;
			else item.PatternTo = paternTo;

			return item;
		} else if (raw.length == 3) {
			let item = new ItemNoun();
			item.From = raw[0];
			item.To = raw[0];

			let paternFrom = this.GetPatternByNameFrom(raw[1]);
			if (paternFrom == null) return null;
			else item.PatternFrom = paternFrom;

			let paternTo = this.GetPatternByNameTo(raw[2]);
			if (paternTo == null) return null;
			else item.PatternTo = paternTo;

			return item;
		} else if (raw.length == 2) {
			let item = new ItemNoun();
			item.From = "";
			item.To = "";

			let paternFrom = this.GetPatternByNameFrom(raw[0]);
			if (paternFrom == null) return null;
			else item.PatternFrom = paternFrom;

			let paternTo = this.GetPatternByNameTo(raw[1]);
			if (paternTo == null) return null;
			else item.PatternTo = paternTo;

			return item;
		}
		return null;
	}

	static GetPatternByNameFrom(name) {
		if (name=="") return null;
		for (const p of this.pattensFrom) {
			if (p.Name==name) return p;
		}
	}
	
	static GetPatternByNameTo(name) {
		if (name=="") return null;
		for (const p of this.pattensTo) {
			if (p.Name==name) return p;
		}
	}

	GetWordTo(number, fall) {
		//if (dev) console.log("GetWordTo",this, number, fall);
		if (this.PatternTo == null) {
			throw Exception(PatternTo+" is null");
			return this.To;
		}
		
	//	console.log("Returning",this, number, fall);
		if (number==undefined) {
			return [this.To+this.PatternTo.Shapes[fall-1], this.PatternTo.Gender];
		}

		if (number==1) {
			return [this.To+this.PatternTo.Shapes[fall-1], this.PatternTo.Gender];
		}
		if (number==2) {
			return [this.To+this.PatternTo.Shapes[fall+6], this.PatternTo.Gender];
		}
		
		
		if (dev) console.log("⚠️ function 'GetWordTo' has unknown parameter 'number' with value '"+number+"'");
		return [this.To+this.PaternTo.Shapes[fall-1], this.PaternTo.Gender];
	}

	GetWordFrom(number, fall) {
		if (this.paternFrom == null) return this.From;

		if (number==1) {
			return [this.From+this.PatternFrom.Shapes[fall-1], this.PatternFrom.Gender];
		}
		if (number==2) {
			return [this.From+this.PatternFrom.Shapes[fall+6], this.PatternFrom.Gender];
		}
		
		if (dev) console.log("⚠️ function 'GetWordTo' has unknown parameter 'number' with value '"+number+"'");
		return [this.From+this.PatternFrom.Shapes[fall-1], this.PatternFrom.Gender];
	}

	

	GetDicForm(name) {		
		if (this.PatternTo.Shapes[0]=="?") return null;	
		return [this.From+this.PatternFrom[0], this.To+this.PatternTo.Shapes[0], this.PatternTo.Gender, name];
	}
	
	/*GetWordTo(number, fall, gender) {
		console.log(number, fall, gender);
		if (gender != this.PatternTo.Gender) return null;
		if (fall==-1) return null;
		console.log(number, fall, gender);
		if (number==1) {
			return [this.To+this.PatternTo.Shapes[fall-1], this.PatternTo.Gender];
		}
		if (number==2) {
			return [this.To+this.PatternTo.Shapes[fall+6], this.PatternTo.Gender];
		}
		if (number==-1) {
			return [[this.To+this.PatternTo.Shapes[fall-1], this.PatternTo.Gender],[this.To+this.PatternTo.Shapes[fall+6], this.PatternTo.Gender]];
		}
		
		console.log("function 'GetWordTo' has unknown parameter 'number' with value '"+number+"'");
		return [this.To+this.PatternTo.Shapes[fall-1], this.PatternTo.Gender];
	}*/
	
	IsStringThisWord(str) {
		if (this.PatternTo===undefined)  return null;
		if (this.PatternFrom===undefined)return null;

		// Return all possible falls with numbers
		// [[tvar, číslo, pád], rod]
		// console.log(str,this);

		if (this.From=="") {
			let ret=[];
			for (let i=0; i<7; i++) {
				let shapes=this.PatternFrom.Shapes[i];

				for (let j=0; j<shapes.length; j++) {
					let shape=shapes[j];
					//console.log(shape);

					if (shape==str) {
						//ret.push(this.To+this.PatternTo.Shapes[i]);
						if (Array.isArray(this.PatternTo.Shapes[i])) {
							for (const z of this.PatternTo.Shapes[i]){
								if (z!='?') ret.push([this.To+z, 1, i+1]); // [tvar, číslo, pád]	
							}
						} else {
							if (shape!="?") ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1]); // [tvar, číslo, pád]	
						}
						break;	
					}
				}
			}

			for (let i=7; i<14; i++) {
				let shapes=this.PatternFrom.Shapes[i];

				for (let j=0; j<shapes.length; j++) {
					let shape=shapes[j];
				//	console.log(shape);

					if (shape==str) {
						//ret.push(this.To+this.PatternTo.Shapes[i]);
						if (Array.isArray(this.PatternTo.Shapes[i])) {
							for (const z of this.PatternTo.Shapes[i]){
								if ('z'!="?") ret.push([this.To+z, 2, i+1-7]); // [tvar, číslo, pád]	
							}
						} else if (this.PatternTo.Shapes[i]!="?") ret.push([this.To+this.PatternTo.Shapes[i], 2, i-7+1]);
						break;
					}
				}
			}

			if (ret.length==0) return null; else return [ret, this.PatternTo.Gender, this];
		} else {	
			if (str.startsWith(this.From)) {
				let ret=[];

				for (let i=0; i<7; i++) {
					let shapes=this.PatternFrom.Shapes[i];

					if (Array.isArray(shapes)) {
						for (const s of shapes) {
							if (this.From+s==str) {
								if (Array.isArray(this.PatternTo.Shapes[i])) {
									for (const z of this.PatternTo.Shapes[i]){
										if (z!="?") ret.push([this.To+z, 1, i+1]); // [tvar, číslo, pád]	
									}
								} else if (this.PatternTo.Shapes[i]!="?") ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1]);
								break;
							}
						}
					} else {
						if (this.From+this.PatternFrom.Shapes[i]==str) {
							if (Array.isArray(this.PatternTo.Shapes[i])) {
								for (const z of this.PatternTo.Shapes[i]){
									if (z!="?") ret.push([this.To+z, 1, i+1]); // [tvar, číslo, pád]	
								}
							} else if (this.PatternTo.Shapes[i]!="?") ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1]);
							break;
						}
					}
				}

				for (let i=7; i<14; i++) {
					let shapes=this.PatternFrom.Shapes[i];

					for (let j=0; j<shapes.length; j++) {
						let shape=this.From+shapes[j];
					//	console.log(shape);

						if (shape==str) {
							//ret.push(this.To+this.PatternTo.Shapes[i]);
							if (Array.isArray(this.PatternTo.Shapes[i])) {
								for (const z of this.PatternTo.Shapes[i]){
									if (z!="?") ret.push([this.To+z, 2, i+1-7]); // [tvar, číslo, pád]	
								}
							} else if (this.PatternTo.Shapes[i]!="?") ret.push([this.To+this.PatternTo.Shapes[i], 2, i-7+1]);
							break;
						}
					}
				}

				if (ret.length==0) return null; else return [ret, this.PatternTo.Gender, this];
			} else {
				return null;
			}
		}
		return null;
	}
}

class ItemSimpleWord {
	constructor() {
		this.input = "";
		this.output=[];
	}
	
	static Load(data) {
		let raw = data.split('|');
		if (raw[0]=='') return null;
		if (raw.length==1){
			if (raw[0].includes('?')) return null;
			let item = new ItemSimpleWord();
			item.input = raw[0];
			item.output = raw[0];
			return item;
		} 
		if (raw.length==2){
			if (raw[0].includes('?')) return null;
			if (raw[1].includes('?')) return null;
			let item = new ItemSimpleWord();
			let inp=raw[0].split(',');
			if (inp.length==1) item.input = inp[0];
			else  item.input = inp;
			item.output = raw[1].split(',');
			return item;
		}
		return null;
    }

	GetDicForm(name) {
		let out = "";
		if (Array.isArray(this.output)) {
			out=this.output.join(", ");
		} else {
			out+=this.output;
		}
		return [this.input, out, "", name];
	}
}

class ItemPhrase {
	constructor() {
		this.input =[]; //[["k", "moři"], ["k", "mořu"]]
		this.output=[]; //[["k", "mořu"], ["k", "mořu"]]
	}
	
	static Load(data) {
		let raw = data.split('|');
		if (raw[0]=='') return null;
		if (raw.length==1) {
			let item = new ItemPhrase();
			item.output = item.input = this.DoubleSplit(data);
			return item;
		} 
		if (raw.length==2){
			let item = new ItemPhrase();
			item.input  = this.DoubleSplit(raw[0]);
			item.output = this.DoubleSplit(raw[1]);
			return item;
		}
		
		return null;
    }

	static DoubleSplit(str) {
		// "k moři,k mořu" ->> [["k", "moři"], ["k", "mořu"]]
		let arr=[];
		for (const w of str.split(",")){
			arr.push(this.MultipleSplit(w," -"));
		}	
		return arr;
	}

	static SplitSentences(string, separators) {
		let arr=[];
		let sentence="";
		let isSeparator;

		for (const ch of string) {
			isSeparator=false;

			// Is current char separator
			for (const s of separators) {
				if (s==ch) {
					isSeparator=true;
					if (sentence!="") {
						sentence+=ch;
						arr.push(sentence.trim());
						sentence="";
					}
					break;
				}
			}

			if (!isSeparator) {
				sentence+=ch;
			}
		}
		if (!isSeparator) {
			if (sentence!="") arr.push(sentence.trim());
		}
		// for example [["true", "He"], [false, " "], [true, "is"], [false, " "], [true, "guy"], [false, "!"]]
		return arr;
	}

	static MultipleSplit(string, separators) {
		let arr=[];
		let word="";
		let isSeparator;

		for (const ch of string) {
			isSeparator=false;
			//let separator;

			// Is current char separator
			for (const s of separators) {
				if (s==ch) {
					isSeparator=true;
					if (word!="") {
						arr.push(/*[true, */word/*]*/);
						word="";
					}
					arr.push(/*[false, */s/*]*/);
					break;
				}
			}

			if (!isSeparator) {
				word+=ch;
			}
		}
		if (!isSeparator) {
			if (word!="") arr.push(/*[true, */word/*]*/);
		}
		// for example [["true", "He"], [false, " "], [true, "is"], [false, " "], [true, "guy"], [false, "!"]]
		return arr;
	}

	GetDicForm(name) {
		let inp = "";
		let out = "";
				
		for (let o of this.output) {
			out+=o.join(" ");
			//for (let o2 of o) o2+" ";
		}
		for (let i of this.input) {
			inp+=i.join(" ");
			//for (let i2 of i) inp+=i2+" ";
		}
		//out=out.substring(0, out.length-1);
		//inp=inp.substring(0, out.length-1);
		
		return [inp, out, "", name];
	}
}

class ItemReplaceS {
	constructor() {
		this.input = "";
		this.output="";
	}
	
	static Load(data) {
		let raw = data.split('|');
		if (raw[0]=='') return null;
		if (raw.length==1){
			let item = new ItemReplaceS();
			item.input = raw[0];
			item.output = raw[0];
			return item;
		} 
		if (raw.length==2){
			let item = new ItemReplaceS();
			item.input  = raw[0];
			item.output = raw[1];
			return item;
		}
		return null;
    }
}

class ItemReplaceG {
	constructor() {
		this.input = "";
		this.output="";
	}
	
	static Load(data) {
		let raw = data.split('|');
		if (raw[0]=='') return null;
		if (raw.length==1){
			let item = new ItemReplaceG();
			item.input = raw[0];
			item.output = raw[0];
			return item;
		} 
		if (raw.length==2){
			let item = new ItemReplaceG();
			item.input  = raw[0];
			item.output = raw[1];
			return item;
		}
		return null;
    }
}

class ItemReplaceE {
	constructor() {
		this.input = "";
		this.output="";
	}
	
	static Load(data) {
		let raw = data.split('|');
		if (raw[0]=='') return null;
		if (raw.length==1){
			let item = new ItemReplaceE();
			item.input = raw[0];
			item.output = raw[0];
			return item;
		} 
		if (raw.length==2) {
			let item = new ItemReplaceE();
			item.input  = raw[0];
			if (raw[1].includes(',')) item.output = raw[1].split(",");
			else item.output = raw[1];
			return item;
		}
		return null;
    }
}

class ItemPreposition {
	constructor() {
		this.input = "";
		this.output = [];
		this.fall=[];
	}
	
	static Load(data) {
		let raw = data.split('|');
		if (raw.length==2){
			if (raw[0].includes('?')) return null;
			let item = new ItemPreposition();
			item.input = raw[0];
			item.output.push(raw[0]);
			if (raw[1]!="") {
				for (const f of raw[1].split(',')) {
					let num=parseInt(f);
					if (!isNaN(num))item.fall.push(num);
				}
			}
			return item;
		} 
		if (raw.length==3){
			if (raw[0].includes('?')) return null;
			if (raw[1].includes('?')) return null;

			let item = new ItemPreposition();
			item.input  = raw[0];
			item.output = raw[1].split(',');
			if (raw[2]!="") {
				for (const f of raw[2].split(',')) {
					let num=parseInt(f);
					if (!isNaN(num)) item.fall.push(num);
				}
			}
			return item;
		}
		return null;
    }

	IsStringThisWord(str) {
		if (this.input==str) {
			if (this.input!="?") return [this.output, this.fall];
		}
		return null;
	}

	GetDicForm(name) {
		if (this.fall.length>0) return [this.input, this.output.join(', '), "pád: "+this.fall.join(', '), name];
		else return [this.input, this.output.join(', '), "", name];
	}
}

class SentencePatternWordSubstitution {
	constructor() {
		this.id = -1;
		this.PartOfspeech=-1; // Podstatná jm., páádavná, ...
		this.Gender=-1;
		this.Fall=-1;
		this.GramaticalNumber=-1;

		this.GenderSameAsId=-1;
		this.FallSameAsId=-1;
		this.GramaticalNumberSameAsId=-1;
	}
}

class SentencePatternWordSubstitutionSimple {
	constructor() {
		this.PartOfspeech=-1; // Podstatná jm., páádavná, ...
		this.Gender=-1;
		this.Fall=-1;
		this.GramaticalNumber=-1;
	}
}

class ItemSentencePattern {
	constructor() {
		this.selectedIndex = 0;
		this.input = [];
		this.output = [];
	}

	static Load(data) {
		let raw = data.split('|');
		if (raw.length==2) {

			let item=new ItemSentencePattern();
			// Input
			// from: Dal jsi to <id=1,3.pád,á.j.,rod muá.>, <id=2,4.pád,á j.,rod muá> <id=3,4.pád,á j.,rod muá>?"
			// from: Dal jsi to <1,13jM>, <id=2,4.pád,á j.,rod muá> <id=3,4.pád,á j.,rod muá>?"
			// to:   Dal jsi to |id=1,3.pád,á.j.,rod muá.|, |id=2,4.pád,á j.,rod muá| |id=3,4.pád,á j.,rod muá|?"
			let inputRawArray = raw[0].split(/<|>/);

			// Vzorec na zaáátku, nebo dál
			let n=0;

			for (let i=0; i<inputRawArray.length; i++) {
			
				if (i%2==n) {
					// sudá (pokud nezaááná)==text
					if (inputRawArray[i] != "") item.input.push(inputRawArray[i]);
				} else {					
					let rawRule=inputRawArray[i];
					let pattern=this.LoadRules(rawRule);
					if (pattern==null) return null;

					item.input.push(pattern);
				}
			}

			let outputRawArray = raw[1].split(/<|>/);
			for (let i=0; i<outputRawArray.length; i++) {
				if (i%2==0) {
					// sudá==text
					if (outputRawArray[i] != "") item.output.push(outputRawArray[i]);
				} else {
					let rawRule=inputRawArray[i];
					let pattern=this.LoadRules(rawRule);
					if (pattern==null) return null;

					item.output.push(pattern);
				}
			}
			return item;
		} else if (raw.length==1) {

			let item=new ItemSentencePattern();
			// Input
			// from: Dal jsi to <id=1,3.pád,á.j.,rod muá.>, <id=2,4.pád,á j.,rod muá> <id=3,4.pád,á j.,rod muá>?"
			// from: Dal jsi to <1,13jM>, <id=2,4.pád,á j.,rod muá> <id=3,4.pád,á j.,rod muá>?"
			// to:   Dal jsi to |id=1,3.pád,á.j.,rod muá.|, |id=2,4.pád,á j.,rod muá| |id=3,4.pád,á j.,rod muá|?"
			let inputRawArray = raw[0].split(/<|>/);

			// Vzorec na zaáátku, nebo dál
			let n=0;

			for (let i=0; i<inputRawArray.length; i++) {
			
				if (i%2==n) {
					// sudá (pokud nezaááná)==text
					if (inputRawArray[i] != "") item.input.push(inputRawArray[i]);
				} else {					
					let rawRule=inputRawArray[i];
					let pattern=this.LoadRules(rawRule);
					if (pattern==null) return null;

					item.input.push(pattern);
				}
			}

			let outputRawArray = raw[0].split(/<|>/);
			for (let i=0; i<outputRawArray.length; i++) {
				if (i%2==0) {
					// sudá==text
					if (outputRawArray[i] != "") item.output.push(outputRawArray[i]);
				} else {
					let rawRule=inputRawArray[i];
					let pattern=this.LoadRules(rawRule);
					if (pattern==null) return null;

					item.output.push(pattern);
				}
			}
			return item;
		}
		return null;
	}

	static LoadRules(rawStr){
		if (rawStr==undefined) return null;
		let pattern=new SentencePatternWordSubstitution();
		let rawRules=rawStr.split(",");

		for (const rawRule of rawRules) {
			
			if (rawRule=="pods") {
				pattern.PartOfspeech=1;
				continue;
			}
			if (rawRule=="prid")  {
				pattern.PartOfspeech=2;
				continue;
			}
			if (rawRule=="zajm")  {
				pattern.PartOfspeech=3;
				continue;
			}
			if (rawRule=="cisl") {
				pattern.PartOfspeech=4;
				continue;
			}
			if (rawRule=="slov") {
				pattern.PartOfspeech=5;
				continue;
			}
			if (rawRule=="pris")  {
				pattern.PartOfspeech=6;
				continue;
			}
			if (rawRule=="pred")  {
				pattern.PartOfspeech=7;
				continue;
			}
			if (rawRule=="spoj")  {
				pattern.PartOfspeech=8;
				continue;
			}
			if (rawRule=="cast") {
				pattern.PartOfspeech=9;
				continue;
			}
			if (rawRule=="cito") {
				pattern.PartOfspeech=10;
				continue;
			}
			
			if (rawRule.includes("=")) {
				let rulerawparts= rawRule.split("=");
				let name=rulerawparts[0];
				let value=rulerawparts[1];
				
				if (name=="id") {
					pattern.id=parseInt(value);
					continue;
				}
				if (name=="pad" || name=="pád") {
					if (value.includes("id")) {
						pattern.FallSameAsId=parseInt(value.substring(2));
					//	console.log(pattern.FallSameAsId,value,value.substring(2));
						continue;
					} else {
						pattern.Fall=parseInt(value);
						continue;
					}
				}
				if (name=="c" || name=="č") {
					if (value.includes("id")) {
						pattern.GramaticalNumberSameAsId =parseInt(value.substring(2));
					//	console.log(pattern.number,value,value.substring(2));
						continue;
					} else {
						if (value=="j") {
							pattern.Number=1;
							continue;
						}
						if (value=="m") {
							pattern.Number=2;
							continue;
						}
						continue;
					}
				}
				if (name=="rod") {
					if (value.includes("id")) {
						pattern.number=parseInt(value.substring(2));
				//		console.log(pattern.number,value,value.substring(2));
						continue;
					} else {
						if (value=="str" || value=="stř") {
							pattern.Gender="str";
							continue;
						}
						if (value=="zen" || value=="žen") {
							pattern.Gender="zen";
							continue;
						}
						if (value=="muz" || value=="muž") {
							pattern.Gender="muz";
							continue;
						}
						if (value=="muz ziv" || value=="muž živ") {
							pattern.Gender="muz ziv";
							continue;
						}
						if (value=="muz nez" || value=="muž než") {
							pattern.Gender="muz nez";
							continue;
						}
					}
				}
			}
			if (dev) console.log("⚠️ Unknows rule in pattern '", rawRule,"' all rules:", rawRules);
			return null;
		}
		return pattern;
	}
	
	MultipleSplit(string) {
		// for example [["true", "...pattern..."], [false, " guy!"]]
		// true  = pattern
		// false = string
		let arr=[];
		let word="";
		let isSeparator;

		for (const ch of string) {
			isSeparator=false;
			let separator;

			// Is current char separator
			if (xh=="<") {
				isSeparator=true;	
				
				if (word!="") {
					arr.push([ch==">", word]);
					word="";
				}
				//arr.push([false, s]);
			} else if (ch==">") {
				isSeparator=true;

				if (word!="") {
					arr.push([ch==">", word]);
					word="";
				}
				//arr.push([true, s]);
			} else if (!isSeparator) {
				word+=ch;
			}
		}

		if (!isSeparator) {
			if (word!="") arr.push([false, word]);
		}
				
		return arr;
	}
}

class ItemSentencePatternPart {
	constructor() {
		this.selectedIndex = 0;
		this.input = [];
		this.output = [];
	}

	static Load(data) {
		let raw = data.split('|');
		if (raw.length==2) {
			let item=new ItemSentencePatternPart();
			// Input
			// from: Dal jsi to <id=1,3.pád,á.j.,rod muá.>, <id=2,4.pád,á j.,rod muá> <id=3,4.pád,á j.,rod muá>?"
			// from: Dal jsi to <1,13jM>, <id=2,4.pád,á j.,rod muá> <id=3,4.pád,á j.,rod muá>?"
			// to:   Dal jsi to |id=1,3.pád,á.j.,rod muá.|, |id=2,4.pád,á j.,rod muá| |id=3,4.pád,á j.,rod muá|?"
			let inputRawArray = raw[0].split(/<|>/);

			// Vzorec na zaáátku, nebo dál
			let n=0;
		//	if (raw[0].startsWith('<')) n=1;

			for (let i=0; i<inputRawArray.length; i++) {
			
				if (i%2==n) {
					// sudá (pokud nezaááná)==text
					if (inputRawArray[i] != "") item.input.push(inputRawArray[i]);
				} else {	//console.log(inputRawArray[i],inputRawArray,n);
					// lichá=pravidlo
					let rawRule=inputRawArray[i];
					let pattern=this.LoadRules(rawRule);
					if (pattern==null) return null;
					//let rawRules=rawRule.split(',');

					//let pattern=new SentencePatternWordSubstitution();
				/*	pattern.id=parseInt(rawRules[0]);

					if (rawRules[1].length==4) {
						// Type
						let rawType=rawRules[1][0];
						if (rawType!='x') pattern.PartOfspeech=parseInt(rawType);

						// Fall
						let rawFall=rawRules[1][1];
						if (rawType!='x') pattern.Fall=parseInt(rawType);

						// Number
						let rawNumber=rawRules[1][2];
						if (rawType=="j") pattern.GramaticalNumber=1;
						else if (rawType=="m") pattern.GramaticalNumber=2;
						
						// Gender
						let rawGender=rawRules[1][3];
						if (rawType=="S") pattern.Gender="S";
						else if (rawType=="Z") pattern.Gender="Z";
						else if (rawType=="M") pattern.Gender="M";
						else if (rawType=="N") pattern.Gender="N";
					} else if (dev) console.log("Error not 4 char match");
					*/
					item.input.push(pattern);
				}
			}

			let outputRawArray = raw[1].split(/<|>/);
			for (let i=0; i<outputRawArray.length; i++) {
				if (i%2==0) {
					// sudá==text
					if (outputRawArray[i] != "") item.output.push(outputRawArray[i]);
				} else {
					// lichá=pravidlo
				//	let rawRule=outputRawArray[i];

					let rawRule=inputRawArray[i];
					let pattern=this.LoadRules(rawRule);
					if (pattern==null) return null;
	/*
					let rawRules=rawRule.split(',');

					let pattern=new SentencePatternWordSubstitution();
					pattern.id=parseInt(rawRules[0]);

					if (rawRules[1].length==4) {
						// Type
						let rawType=rawRules[1][0];
						if (rawType!='x') pattern.PartOfspeech=parseInt(rawType);

						// Fall
						let rawFall=rawRules[1][1];
						if (rawType!='x') pattern.Fall=parseInt(rawType);

						// Number
						let rawNumber=rawRules[1][2];
						if (rawType=="j") pattern.GramaticalNumber=1;
						else if (rawType=="m") pattern.GramaticalNumber=2;
						
						// Gender
						let rawGender=rawRules[1][3];
						if (rawType=="S") pattern.Gender="S";
						else if (rawType=="Z") pattern.Gender="Z";
						else if (rawType=="M") pattern.Gender="M";
						else if (rawType=="N") pattern.Gender="N";
					} else if (dev) console.log("Error not 4 char match");*/

					item.output.push(pattern);
				}
			}
			return item;
		}
	}

	static LoadRules(rawStr){
		let pattern=new SentencePatternWordSubstitution();
		let rawRules=rawStr.split(",");

		for (const rawRule of rawRules) {
			
			if (rawRule=="pods") {
				pattern.PartOfspeech=1;
				continue;
			}
			if (rawRule=="prid")  {
				pattern.PartOfspeech=2;
				continue;
			}
			if (rawRule=="zajm")  {
				pattern.PartOfspeech=3;
				continue;
			}
			if (rawRule=="cisl") {
				pattern.PartOfspeech=4;
				continue;
			}
			if (rawRule=="slov") {
				pattern.PartOfspeech=5;
				continue;
			}
			if (rawRule=="pris")  {
				pattern.PartOfspeech=6;
				continue;
			}
			if (rawRule=="pred")  {
				pattern.PartOfspeech=7;
				continue;
			}
			if (rawRule=="spoj")  {
				pattern.PartOfspeech=8;
				continue;
			}
			if (rawRule=="cast") {
				pattern.PartOfspeech=9;
				continue;
			}
			if (rawRule=="cito") {
				pattern.PartOfspeech=10;
				continue;
			}
			
			if (rawRule.includes("=")) {
				let rulerawparts= rawRule.split("=");
				let name=rulerawparts[0];
				let value=rulerawparts[1];
				
				if (name=="id") {
					pattern.id=parseInt(value);
					continue;
				}
				if (name=="pad" || name=="pád") {
					if (value.includes("id")) {
						pattern.FallSameAsId=parseInt(value.substring(2));
					//	console.log(pattern.FallSameAsId,value,value.substring(2));
						continue;
					} else {
						pattern.Fall=parseInt(value);
						continue;
					}
				}
				if (name=="c" || name=="č") {
					if (value.includes("id")) {
						pattern.GramaticalNumberSameAsId =parseInt(value.substring(2));
					//	console.log(pattern.number,value,value.substring(2));
						continue;
					} else {
						if (value=="j") {
							pattern.Number=1;
							continue;
						}
						if (value=="m") {
							pattern.Number=2;
							continue;
						}
						continue;
					}
				}
				if (name=="rod") {
					if (value.includes("id")) {
						pattern.number=parseInt(value.substring(2));
				//		console.log(pattern.number,value,value.substring(2));
						continue;
					} else {
						if (value=="str" || value=="stř") {
							pattern.Gender="str";
							continue;
						}
						if (value=="zen" || value=="žen") {
							pattern.Gender="zen";
							continue;
						}
						if (value=="muz" || value=="muž") {
							pattern.Gender="muz";
							continue;
						}
						if (value=="muz ziv" || value=="muž živ") {
							pattern.Gender="muz ziv";
							continue;
						}
						if (value=="muz nez" || value=="muž než") {
							pattern.Gender="muz nez";
							continue;
						}
					}
				}
			}
			if (dev) console.log("⚠️ Unknown rule in pattern '", rawRule,"' all rules:", rawRules);
			return null;
		}
		return pattern;
	}
	
	MultipleSplit(string) {
		// for example [["true", "...pattern..."], [false, " guy!"]]
		// true  = pattern
		// false = string
		let arr=[];
		let word="";
		let isSeparator;

		for (const ch of string) {
			isSeparator=false;
			let separator;

			// Is current char separator
			if (xh=="<") {
				isSeparator=true;	
				
				if (word!="") {
					arr.push([ch==">", word]);
					word="";
				}
				//arr.push([false, s]);
			} else if (ch==">") {
				isSeparator=true;

				if (word!="") {
					arr.push([ch==">", word]);
					word="";
				}
				//arr.push([true, s]);
			} else if (!isSeparator) {
				word+=ch;
			}
		}

		if (!isSeparator) {
			if (word!="") arr.push([false, word]);
		}
				
		return arr;
	}
}

class ItemPatternPronoun{
	constructor() {
		this.Name;
		this.Gender;
		this.Type;
		this.Shapes;
	}

	static Load(data) {
		let raw=data.split('|');
		if (raw.length==14+2){
			let item=new ItemPatternPronoun();
			item.Name=raw[0];
			item.Type=1;
			item.Gender=parseInt(raw[1]);
			item.Shapes=[14];
			for (let i=0; i<14; i++){
				if (raw[2+i].includes('?')) item.Shapes[i]='?';
				else item.Shapes[i]=raw[2+i].split(',');
			}
			return item; 
		}
		if (raw.length==7+2){
			let item=new ItemPatternPronoun();
			item.Name=raw[0];
			item.Type=2;
			item.Gender=parseInt(raw[1]);
			item.Shapes=[7];
			for (let i=0; i<7; i++){
				if (raw[2+i].includes('?')) item.Shapes[i]='?';
				else item.Shapes[i]=raw[2+i].split(',');
			}
			return item; 
		}
		if (raw.length==1+2){
			let item=new ItemPatternPronoun();
			item.Name=raw[0];
			item.Type=3;
			item.Gender=parseInt(raw[1]);
			item.Shapes=[raw[2]].split(',');
			if (raw[2].includes('?')) item.Shapes[i]='?';
			return item; 
		}
		if (raw.length==14*4+2){
			let item=new ItemPatternPronoun();
			item.Name=raw[0];
			item.Type=4;
			item.Gender=parseInt(raw[1]);
			item.Shapes=[14*4];
			for (let i=0; i<14*4; i++) {
				if (raw[2+i].includes('?')) item.Shapes[i]='?';
				else item.Shapes[i]=raw[2+i].split(',');
			}
			return item; 
		}
		if (dev) console.log("⚠️ PatternPronoun - Chybná délka");
		return null;
	}
} 

class ItemPronoun{
	static pattensFrom=[];
	static pattensTo=[];

	constructor() {
		this.From;
		this.To;
		this.PatternFrom;
		this.PatternTo;
	}

	static Load(data) {
		let raw=data.split('|');
		if (raw.length==4) {
			let item = new ItemPronoun();
			item.From=raw[0];
			item.To=raw[1];
			
			let paternFrom = this.GetPatternByNameFrom(raw[2]);
			if (paternFrom == null) return null;
			else item.PatternFrom = paternFrom;
			
			let paternTo = this.GetPatternByNameTo(raw[3]);	
			if (paternTo == null) return null;
			else item.PatternTo = paternTo;

			return item;
		}
		if (raw.length==3) {
			let item = new ItemPronoun();
			item.From=raw[0];
			item.To=raw[0];
			
			let paternFrom = this.GetPatternByNameFrom(raw[1]);
			if (paternFrom == null) return null;
			else item.PatternFrom = paternFrom;
			
			let paternTo = this.GetPatternByNameTo(raw[2]);	
			if (paternTo == null) return null;
			else item.PatternTo = paternTo;

			return item;
		}
		if (raw.length==2) {
			let item =new ItemPronoun();
			item.From="";
			item.To="";
			
			let paternFrom = this.GetPatternByNameFrom(raw[0]);
			if (paternFrom == null) return null;
			else item.PatternFrom = paternFrom;
			
			let paternTo = this.GetPatternByNameTo(raw[1]);	
			if (paternTo == null) return null;
			else item.PatternTo = paternTo;

			return item;
		}
		return null;
	}

	static GetPatternByNameFrom(name) {
		for (const p of this.pattensFrom) {
			if (p.Name==name) return p;
		}
	}

	static GetPatternByNameTo(name) {
		for (const p of this.pattensTo) {
			if (p.Name==name) return p;
		}
	}

	IsStringThisWord(str) {
		// Return all possible falls with numbers
		// [[tvar, číslo, pád], rod]
	//	console.log(str);
		if (!str.startsWith(this.From)) return null;
	//	console.log(this.PatternFrom.Shapes.length);
		
		let ret=[];
		if (this.PatternFrom.Shapes.length==14*4) {
			for (let i=0; i<7; i++) {
				let shapes=this.PatternFrom.Shapes[i];
				
				for (const s of shapes) {
					if (this.From+s==str) {
						let arr=[];
						for (let s of this.PatternTo.Shapes[i]) {
							if (s!="?") arr.push([this.To+s]);
						}
						if (arr.length>0)ret.push([arr, 1, i+1,"muz"]);
						break;
					}
				}
			}
			for (let i=7; i<14; i++) {
				let shapes=this.PatternFrom.Shapes[i];

				for (let j=0; j<shapes.length; j++) {
					let shape=this.From+shapes[j];

					if (shape==str) {
						if (this.PatternTo.Shapes[i]!="?") ret.push([this.To+this.PatternTo.Shapes[i], 2, i-7+1,"muz"]);
						break;
					}
				}
			}

			for (let i=14; i<21; i++) {
				let shapes=this.PatternFrom.Shapes[i];
				
				for (let j=0; j<shapes.length; j++) {
					let shape=this.From+shapes[j];
					//console.log(this.From+shapes[j]);
					if (shape==str) {
						if (this.PatternTo.Shapes[i]!="?") ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1-14,"mun"]);
						break;
					}
				}
			}
			for (let i=21; i<28; i++) {
				let shapes=this.PatternFrom.Shapes[i];

				for (let j=0; j<shapes.length; j++) {
					let shape=this.From+shapes[j];

					if (shape==str) {
						if (this.PatternTo.Shapes[i]!="?") ret.push([this.To+this.PatternTo.Shapes[i], 2, i-7-14+1,"mun"]);
						break;
					}
				}
			}

			for (let i=28; i<35; i++) {
				let shapes=this.PatternFrom.Shapes[i];
				
				for (let j=0; j<shapes.length; j++) {
					let shape=this.From+shapes[j];
					//console.log(this.From+shapes[j]);
					if (shape==str) {
						if (this.PatternTo.Shapes[i]!="?") ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1-14-14,"zen"]);
						break;
					}
				}
			}
			for (let i=35; i<28+14; i++) {
				let shapes=this.PatternFrom.Shapes[i];

				for (let j=0; j<shapes.length; j++) {
					let shape=this.From+shapes[j];

					if (shape==str) {
						if (this.PatternTo.Shapes[i]!="?") ret.push([this.To+this.PatternTo.Shapes[i], 2, i-7-14-14+1,"zen"]);
						break;
					}
				}
			}

			for (let i=14+14+14; i<21+14+14; i++) {
				let shape=this.PatternFrom.Shapes[i];
				
				for (const s of shape) {
					if (this.From+s==str) {
						if (this.PatternTo.Shapes[i]!="?") ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1-14-14-14, "str"]);
						break;
					}
				}
			}
			for (let i=21+14+14; i<28+14+14; i++) {
				let shapes=this.PatternFrom.Shapes[i];

				for (let j=0; j<shapes.length; j++) {
					let shape=this.From+shapes[j];

					if (shape==str) {
						if (this.PatternTo.Shapes[i]!="?") ret.push([this.To+this.PatternTo.Shapes[i], 2, i-7-14+1-14-14, "str"]);
						break;
					}
				}
			}

			if (ret.length==0) return null; else return [ret, this.PatternTo.Gender, this];
		}else if (this.PatternFrom.Shapes.length==14) {
			for (let i=0; i<7; i++) {
				let shapes=this.PatternFrom.Shapes[i];
				
				for (let j=0; j<shapes.length; j++) {
					let shape=this.From+shapes[j];
//					console.log(this.From+shapes[j]);
					if (shape==str) {
						let arr=[];
						for (let s of this.PatternTo.Shapes[i]) {
							if (s!="?") arr.push(this.To+s);
						}
						if (arr.length>0)ret.push([arr, 1, i+1]);
						break;
					}
				}
			}
			for (let i=7; i<14; i++) {
				let shapes=this.PatternFrom.Shapes[i];

				for (let j=0; j<shapes.length; j++) {
					let shape=this.From+shapes[j];

					if (shape==str) {
						let arr=[];
						for (let s of this.PatternTo.Shapes[i]) {
							if (s!="?") arr.push(this.To+s);
						}
						if (arr.length>0)ret.push([arr, 2, i-7+1]);
						break;
					}
				}
			}

			if (ret.length==0) return null; else return [ret, this.PatternTo.Gender, this];
		}else

		if (this.PatternFrom.Shapes.length==7) {
			for (let i=0; i<7; i++) {
				let shapes=this.PatternFrom.Shapes[i];

				for (let j=0; j<shapes.length; j++) {
					let shape=this.From+shapes[j];
					//console.log(this.From+shapes[j]);
					if (shape==str) {
						let arr=[];
						for (let s of this.PatternTo.Shapes[i]) {
							if (s!="?") arr.push(this.To+s);
						}
						if (arr.length>0)ret.push([arr, -1, i+1]);
						break;
					}
				}
			}

			if (ret.length==0) return null; else return [ret, this.PatternTo.Gender, this];
		}else
		
		if (this.PatternFrom.Shapes.length==1) {
			//for (let i=0; i<7; i++) {
				let shapes=this.PatternFrom.Shapes[0];

				//for (let j=0; j<shapes.length; j++) {
					let shape=this.From+shapes[0];
					if (shape==str) {
						if (this.PatternTo.Shapes[0]!="?") ret.push([this.To+this.PatternTo.Shapes[0], -1,-1]);
						//break;
					}
				//}
			//}

			if (ret.length==0) return null; else return [ret, this.PatternTo.Gender, this];
		}
	
		return null;
	}

	GetDicForm(name) {
		if (this.PatternTo.Shapes[0]=="?") return null;
		return [this.From+this.PatternFrom.Shapes[0], this.To+this.PatternTo.Shapes[0], "", name];
	}
} 

class ItemPatternAdjective{
	constructor() {
		this.Name;
		this.adjectiveType;
		this.Middle=[];
		this.Feminine=[];
		this.MasculineAnimate=[];
		this.MasculineInanimate=[];
	}

	static Load(data) {
		let raw=data.split('|');
		if (raw.length!=14*4+2) {
			if (dev) console.log("PatternPronoun - Chybná délka");
			return null;
		}
		let item=new ItemPatternAdjective();
		item.Name=raw[0];
		item.adjectiveType=parseInt(raw[1]);
		item.Middle             =[ raw[2],  raw[3],  raw[4],  raw[5],  raw[6],  raw[7],   raw[8],  raw[9],  raw[10], raw[11], raw[12], raw[13], raw[14], raw[15]];
		item.Feminine           =[ raw[16], raw[17], raw[18], raw[19], raw[20], raw[21],  raw[22], raw[23], raw[24], raw[25], raw[26], raw[27], raw[28], raw[29]];
		item.MasculineAnimate   =[ raw[30], raw[31], raw[32], raw[33], raw[34], raw[35],  raw[36], raw[37], raw[38], raw[39], raw[40], raw[41], raw[42], raw[43]];
		item.MasculineInanimate =[ raw[44], raw[45], raw[46], raw[47], raw[48], raw[49],  raw[50], raw[51], raw[52], raw[53], raw[54], raw[55], raw[56], raw[57]];
		return item;
	}
} 

class ItemAdjective{
	static pattensFrom;
	static pattensTo;

	constructor() {
		this.From;
		this.To;
		this.PatternFrom;
		this.PatternTo;
	}
	
	static Load(data) {
		let raw=data.split('|');
		if (raw.length==4) { 
			let paternFrom = this.GetPatternByNameFrom(raw[2]);
			if (paternFrom == null) {
				if (dev) console.log("Cannot load pattern '"+raw[2]+"'");
				return null;
			}

			let paternTo = this.GetPatternByNameTo(raw[3]);
			if (paternTo == null) {
				if (dev) console.log("Cannot load pattern '"+raw[3]+"'");
				return null;
			}

			let item =new ItemAdjective();
			item.From=raw[0];
			item.To=raw[1];
			item.PatternFrom=paternFrom;
			item.PatternTo=paternTo;
			return item;
		} else if (raw.length==3) { 
			let paternFrom = this.GetPatternByNameFrom(raw[1]);
			if (paternFrom == null) {
				if (dev) console.log("Cannot load pattern '"+raw[1]+"'");
				return null;
			}

			let paternTo = this.GetPatternByNameTo(raw[2]);
			if (paternTo == null) {
				if (dev) console.log("Cannot load pattern '"+raw[2]+"'");
				return null;
			}

			let item =new ItemAdjective();
			item.From=raw[0];
			item.To=raw[0];
			item.PatternFrom=paternFrom;
			item.PatternTo=paternTo;
			return item;
		}
		if (dev) console.log("Cannot load pattern, wrong len");
		return null;
	}
	IsStringThisWord(str) {
		// Return all possible falls with numbers
		// [[tvar, číslo, pád], rod]
		if (this.From=="") {
			let ret=[];
			for (let i=0; i<7; i++) {
				let shape=this.PatternFrom.Feminine[i];

			//	for (let j=0; j<shapes.length; j++) {
					//let shape=shapes[j];
				//	console.log(shape);

					if (shape==str) {
						//ret.push(this.To+this.PatternTo.Shapes[i]);	
						if (this.PatternTo.Feminine[i]!="?") ret.push([this.To+this.PatternTo.Feminine[i], 1, i+1, "Feminine"]); // [tvar, rod, číslo, pád]	
						break;	
					}
			//	}
			}

			for (let i=7; i<14; i++) {
				let shape=this.PatternFrom.Feminine[i];

			//	for (let j=0; j<shapes.length; j++) {
					//let shape=shapes[j];
				//	console.log(shape);

					if (shape==str) {
						//ret.push(this.To+this.PatternTo.Shapes[i]);
						
						if (this.PatternTo.Feminine[i]!="?") ret.push([this.To+this.PatternTo.Feminine[i], 2, i-7+1, "Feminine"]);
						break;
					}
				//}
			}

			for (let i=0; i<7; i++) {
				let shape=this.PatternFrom.MasculineAnimate[i];

			//	for (let j=0; j<shapes.length; j++) {
				//	let shape=shapes[j];
				//	console.log(shape);

					if (shape==str) {
						//ret.push(this.To+this.PatternTo.Shapes[i]);	
						if (this.PatternTo.MasculineAnimate[i]!="?") ret.push([this.To+this.PatternTo.MasculineAnimate[i], 1, i+1, "MasculineAnimate"]); // [tvar, rod, číslo, pád]	
						break;	
					}
				//}
			}

			for (let i=7; i<14; i++) {
				let shape=this.PatternFrom.MasculineAnimate[i];

				//for (let j=0; j<shapes.length; j++) {
				//	let shape=shapes[j];
					//console.log(shape);

					if (shape==str) {
						//ret.push(this.To+this.PatternTo.Shapes[i]);
						if (this.PatternTo.MasculineAnimate[i]!="?") ret.push([this.To+this.PatternTo.MasculineAnimate[i], 2, i-7+1, "MasculineAnimate"]);
						break;
					}
			//	}
			}

			if (ret.length==0) return null; else return ret;
		} else {
			if (str.startsWith(this.From)) {
				let ret=[];

				for (let i=0; i<7; i++) {
					let shape=this.From+this.PatternFrom.Feminine[i];
					
				//	for (let j=0; j<shapes.length; j++) {
					//	let shape=this.From+shapes[j];
						//console.log("Feminine", shape);
						
						if (shape==str) {
							//ret.push(this.To+this.PatternTo.Shapes[i]);
							if (this.PatternTo.Femimine[i]!="?") {
								 ret.push([this.To+this.PatternTo.Feminine[i], 1, i+1, "Feminine"]);
								 break;
							}
						}
				//	}
				}

				for (let i=7; i<14; i++) {
					let shape=this.From+this.PatternFrom.Feminine[i];

					//for (let j=0; j<shapes.length; j++) {
						//let shape=this.From+shapes[j];
						//console.log("Feminine", shape);
					//	console.log(this.PatternTo.Feminine);
						if (shape==str) {
							//ret.push(this.To+this.PatternTo.Shapes[i]);
							if (this.PatternTo.Feminine[i]!="?") ret.push([this.To+this.PatternTo.Feminine[i], 2, i-7+1, "Feminine"]);
							break;
						}
					//}
				}

				for (let i=0; i<7; i++) {
					let shape=this.From+this.PatternFrom.MasculineAnimate[i];
					
				//	for (let j=0; j<shapes.length; j++) {
					//	let shape=this.From+shapes[j];
						//console.log("MasculineAnimate", shape);
						
						if (shape==str) {
							//ret.push(this.To+this.PatternTo.Shapes[i]);
							if (this.PatternTo.MasculineAnimate[i]!="?") ret.push([this.To+this.PatternTo.MasculineAnimate[i], 1, i+1, "MasculineAnimate"]);
							break;
						}
					//}
				}

				for (let i=7; i<14; i++) {
					let shape=this.From+this.PatternFrom.MasculineAnimate[i];

					//for (let j=0; j<shapes.length; j++) {
					//	let shape=this.From+shapes[j];
						//console.log("MasculineAnimate",shape);

						if (shape==str) {
							//ret.push(this.To+this.PatternTo.Shapes[i]);
							if (this.PatternTo.MasculineAnimate[i]!="?") ret.push([this.To+this.PatternTo.MasculineAnimate[i], 2, i-7+1, "MasculineAnimate"]);
							break;
						}
				//	}
				}

				for (let i=0; i<7; i++) {
					let shape=this.From+this.PatternFrom.MasculineInanimate[i];
					
				//	for (let j=0; j<shapes.length; j++) {
					//	let shape=this.From+shapes[j];
					//	console.log(shape);
						
						if (shape==str) {
							//ret.push(this.To+this.PatternTo.Shapes[i]);
							if (this.PatternTo.MasculineInanimate[i]!="?") ret.push([this.To+this.PatternTo.MasculineInanimate[i], 1, i+1, "MasculineInanimate"]);
							break;
						}
				//}
				}

				for (let i=7; i<14; i++) {
					let shape=this.From+this.PatternFrom.MasculineInanimate[i];

				//	for (let j=0; j<shapes.length; j++) {
					//	let shape=this.From+shapes[j];
					//	console.log(shape);

						if (shape==str) {
							//ret.push(this.To+this.PatternTo.Shapes[i]);
							if (this.PatternTo.MasculineInanimate[i]!="?") ret.push([this.To+this.PatternTo.MasculineInanimate[i], 2, i-7+1, "MasculineInanimate"]);
							break;
						}
					//}
				}

				for (let i=0; i<7; i++) {
					let shape=this.From+this.PatternFrom.Middle[i];
					
				//	for (let j=0; j<shapes.length; j++) {
					//	let shape=this.From+shapes[j];
					//	console.log(shape);
						
						if (shape==str) {
							//ret.push(this.To+this.PatternTo.Shapes[i]);
							if (this.PatternTo.Middle[i]!="?") ret.push([this.To+this.PatternTo.Middle[i], 1, i+1, "Middle"]);
							break;
						}
				//}
				}

				for (let i=7; i<14; i++) {
					let shape=this.From+this.PatternFrom.Middle[i];

				//	for (let j=0; j<shapes.length; j++) {
					//	let shape=this.From+shapes[j];
					//	console.log(shape);

						if (shape==str) {
							//ret.push(this.To+this.PatternTo.Shapes[i]);
							if (this.PatternTo.Middle[i]!="?") ret.push([this.To+this.PatternTo.Middle[i], 2, i-7+1, "Middle"]);
							break;
						}
					//}
				}
				if (ret.length==0) return null; else return ret;
			} else {
				return null;
			}
		}
		return null;
	}

	static GetPatternByNameFrom(name) {
		for (const p of this.pattensFrom) {
			if (p.Name==name) return p;
		}
		return null;
	}

	static GetPatternByNameTo(name) {
		for (const p of this.pattensTo) {
			if (p.Name==name) return p;
		}
		return null;
	}

	GetDicForm(name) {	
		if (this.PatternTo.Shapes[0]=="?") return null;	
		return [this.From+this.PatternFrom[0], this.To+this.PatternTo[0], "", name];
	}
}

class ItemPatternNumber{
	constructor() {
		this.Name;
		this.Shapes=[];
	}
		
	static Load(data) {
		let raw=data.split('|');

		let item=new ItemPatternNumber();
		item.Name=raw[0];
		//item.Gender=parseInt(raw[1]);
		
		if (raw.length==14+2) {
			for (let i=0; i<14; i++) { 
				if (raw[2+i].includes('?'))item.Shapes.push('?'); 
				else item.Shapes.push(raw[2+i].split(',')); 
			}
		} else if (raw.length==7+2) {
			for (let i=0; i<7; i++) { 
				if (raw[2+i].includes('?'))item.Shapes.push('?'); 
				else item.Shapes.push(raw[2+i].split(',')); 
			}
		} else if (raw.length==1+2) {
			if (raw[2].includes('?'))item.Shapes.push('?'); 
			else item.Shapes =[raw[2].split(',')];
		}else if (raw.length==14*4+2) {
			for (let i=0; i<14*4; i++) { 
				if (raw[2+i].includes('?'))item.Shapes.push('?'); 
				else item.Shapes.push(raw[2+i].split(',')); 
			}
		}else return null;


		return item;
	}
} 

class ItemNumber{
	static PatternFrom=[];
	static PatternTo=[];

	constructor() {
		this.From; 
		this.PatternFrom; 
		this.To;
		this.PatternTo;
	}
	
	static Load(data) {
		let raw=data.split('|');
		if (raw.length==4) { 
			let item =new ItemNumber();
			item.From=raw[0];
			item.To=raw[1];

			let paternFrom = this.GetPatternByNameFrom(raw[2]);
			if (paternFrom == null) return null;
			else item.PatternFrom = paternFrom;

			let paternTo = this.GetPatternByNameTo(raw[3]);	
			if (paternFrom == null) return null;
			else item.PatternTo = paternTo;

			return item;
		}
		return null;
	}

	static GetPatternByNameFrom(name) {
		for (const p of this.pattensFrom) {
			if (p.Name==name) return p;
		}
	}

	static GetPatternByNameTo(name) {
		for (const p of this.pattensTo) {
			if (p.Name==name) return p;
		}
	}

	IsStringThisWord(str) {
		if (this.PatternTo==null) return;
		if (this.PatternFrom==null) return;

		// Return all possible falls with numbers
		// [[tvar, číslo, pád], rod]
		if (!str.startsWith(this.From)) return;
		let ret=[];

		for (let i=0; i<7; i++) {
			let shape=this.PatternFrom.Shapes[i];
			
			if (Array.isArray(shape)) {
				for (const s of shape) {
					if (this.From+s==str) {
						if (this.PatternTo.Shapes[i]!="?") ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1, "muz"]);
						break;
					}
				}
			} else {
				if (this.From+shape==str) {
					ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1, "muz"]);
					break;
				}
			}
		}

		for (let i=7; i<14; i++) {
			let shape=this.PatternFrom.Shapes[i];

			if (Array.isArray(shape)) {
				for (const s of shape) {
					if (this.From+s==str) {
						ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1, "mun"]);
						break;
					}
				}
			} else {
				if (this.From+shape==str) {
					ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1, "mun"]);
					break;
				}
			}
		}
		
		for (let i=0+14; i<7+14; i++) {
			let shape=this.PatternFrom.Shapes[i];

			if (Array.isArray(shape)) {
				for (const s of shape) {
					if (this.From+s==str) {
						ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1, "mun"]);
						break;
					}
				}
			} else {
				if (this.From+shape==str) {
					ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1, "mun"]);
					break;
				}
			}
		}

		for (let i=7+14; i<14+14; i++) {
			let shape=this.PatternFrom.Shapes[i];

			if (Array.isArray(shape)) {
				for (const s of shape) {
					if (this.From+s==str) {
						ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1-7, "mun"]);
						break;
					}
				}
			} else {
				if (this.From+shape==str) {
					ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1-7, "mun"]);
					break;
				}
			}
		}
		
		for (let i=0+14+14; i<7+14+14; i++) {
			let shape=this.PatternFrom.Shapes[i];

			if (Array.isArray(shape)) {
				for (const s of shape) {
					if (this.From+s==str) {
						ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1, "zen"]);
						break;
					}
				}
			} else {
				if (this.From+shape==str) {
					ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1, "zen"]);
					break;
				}
			}
		}

		for (let i=7+14+14; i<14+14+14; i++) {
			let shape=this.PatternFrom.Shapes[i];

			if (Array.isArray(shape)) {
				for (const s of shape) {
					if (this.From+s==str) {
						ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1-7, "zen"]);
						break;
					}
				}
			} else {
				if (this.From+shape==str) {
					ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1-7, "zen"]);
					break;
				}
			}
		}
		
		for (let i=0+14+14+14; i<7+14+14+14; i++) {
			let shape=this.PatternFrom.Shapes[i];

			if (Array.isArray(shape)) {
				for (const s of shape) {
					if (this.From+s==str) {
						ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1, "str"]);
						break;
					}
				}
			} else {
				if (this.From+shape==str) {
					ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1, "str"]);
					break;
				}
			}
		}

		for (let i=7+14+14+14; i<14+14+14+14; i++) {
			let shape=this.PatternFrom.Shapes[i];
			if (Array.isArray(shape)) {
				for (const s of shape) {
					if (this.From+s==str) {
						ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1-7, "str"]);
						break;
					}
				}
			} else {
				if (this.From+shape==str) {
					ret.push([this.To+this.PatternTo.Shapes[i], 1, i+1-7, "str"]);
					break;
				}
			}
		}

		if (ret.length==0) return null; else return [ret, this];
	}
	
	GetDicForm(name) {	
		if (this.PatternTo.Shapes[0]=="?") return null;	
		return [this.From+this.PatternFrom[0], this.To+this.PatternTo[0], "", name];
	}
} 

class ItemPatternVerb{
	constructor() {
		this.Name;
		this.Infinitive="";
		//this.Type;
		//this.Continous=[];
		//this.Future=[];
		//this.Imperative=[];
		//this.PastActive=[];
		//this.PastPassive=[];
		//this.Transgressive=[];
		//this.Auxiliary=[];
	}	
	
	static GetArray(source, pos, len) { 
		let arr = [len];
		for (let i=0; i<len; i++) {			
			if (source[pos+i].includes(",")) {
				arr[i]=[];
				for (let f of source[pos+i].split(',')) {
					if (f.includes('?')) arr[i].push('?'); 
					else arr[i].push(f); 
				}
			}//arr[i]=source[pos+i].split(',');
			else if (source[pos/*2*/+i].includes('?')) arr[i]='?'; 
			else arr[i]=source[pos+i];
		}
		return arr;
	}
	
	static Load(data) {
		let raw=data.split('|');
		let item=new ItemPatternVerb();
		item.Name=raw[0];
		//item.TypeShow=parseInt(raw[1]);
		let num=parseInt(raw[1]);
		item.Type=parseInt(raw[2]);
		item.Infinitive=raw[3];
		let index=4;
		let SContinous          = (num &   1) ==  1;
		let SImperative         = (num &   2) ==  2;
		let SPastActive         = (num &   4) ==  4;
		let SPastPassive        = (num &   8) ==  8;
		let SFuture             = (num &  16) == 16;
		let STransgressiveCont  = (num &  32) == 32;
		let STransgressivePast  = (num &  64) == 64;
		let SAuxiliary          = (num & 128) ==128;
	//	try {
			if (SContinous) {	    item.Continous 		 	= this.GetArray(raw, index, 6); index+=6;}
			if (SFuture) {      	item.Future 			= this.GetArray(raw, index, 6); index+=6;}
			if (SImperative) {      item.Imperative 		= this.GetArray(raw, index, 3); index+=3;}
			if (SPastActive) {      item.PastActive 		= this.GetArray(raw, index, 8); index+=8;}
			if (SPastPassive) {     item.PastPassive 	 	= this.GetArray(raw, index, 8); index+=8;}
			if (STransgressiveCont) {item.TransgressiveCont	= this.GetArray(raw, index, 3); index+=3;}
			if (STransgressivePast) {item.TransgressivePast	= this.GetArray(raw, index, 3); index+=3;}
			if (SAuxiliary) {        item.Auxiliary 		= this.GetArray(raw, index, 6); index+=6;}
//			console.log(item);
			return item;
	//} catch {
	//		return null;
	//	}
		/*	let index=4;
			if (item.TypeShow==4 || item.TypeShow==0) { 
				item.Infinitive=raw[3];
				item.Continous      = this.GetArray(raw, index, 6); index+=6;
				item.Future         = this.GetArray(raw, index, 6); index+=6;
				item.Imperative     = this.GetArray(raw, index, 3); index+=3;
				item.PastActive     = this.GetArray(raw, index, 8); index+=8;
				item.PastPassive    = this.GetArray(raw, index, 8); index+=8;
				item.Transgressive  = this.GetArray(raw, index, 6); index+=6;
				item.Auxiliary      = this.GetArray(raw, index, 6); index+=6;
			} else if (item.TypeShow==1) { 
				item.Infinitive=raw[3];
				item.Continous      = this.GetArray(raw, index, 6); index+=6;
				item.Imperative     = this.GetArray(raw, index, 3); index+=3;
				item.PastPassive    = this.GetArray(raw, index, 8); index+=8;
				item.Transgressive  = this.GetArray(raw, index, 6); index+=6;
			} else if (item.TypeShow==2) { 
				item.Infinitive=raw[3];
				item.Continous      = this.GetArray(raw, index, 6); index+=6;
				item.Imperative     = this.GetArray(raw, index, 3); index+=3;
				item.PastActive     = this.GetArray(raw, index, 8); index+=8;
				item.Transgressive  = this.GetArray(raw, index, 6); index+=6;
			} else if (item.TypeShow==3) { 
				item.Infinitive=raw[3];
				item.Continous      = this.GetArray(raw, index, 6); index+=6;
				item.Imperative     = this.GetArray(raw, index, 3); index+=3;
				item.PastActive     = this.GetArray(raw, index, 8); index+=8;
				item.PastPassive    = this.GetArray(raw, index, 8); index+=8;
				item.Transgressive  = this.GetArray(raw, index, 6); index+=6;
			} else {
				//throw new Exception("Unknown ShowType");
				return null;
			}*/
		return item;
	}
} 

class ItemVerb{
	
	static pattensFrom=[];
	static pattensTo=[];

	constructor() {
		this.From;
		this.To;
		this.PatternFrom; 
		this.PatternTo;
	}
	
	static Load(data) {
		let raw=data.split('|');
		if (raw.length==4) { 
			let paternFrom = this.GetPatternByNameFrom(raw[2]);
			if (paternFrom == null) {
				if (dev) console.log("Cannot load pattern '"+raw[2]+"'");
				return null;
			}
	
			let paternTo = this.GetPatternByNameTo(raw[3]);
			if (paternTo == null) {
				if (dev) console.log("Cannot load pattern '"+raw[3]+"'");
				return null;
			}
			let item =new ItemVerb();
			item.From=raw[0];
			item.To=raw[1];
			item.PatternFrom=paternFrom;
			item.PatternTo=paternTo;
			return item;
		}
		if (raw.length==3) { 
			let paternFrom = this.GetPatternByNameFrom(raw[1]);
			if (paternFrom == null) {
				if (dev) console.log("Cannot load pattern '"+raw[1]+"'");
				return null;
			}
	
			let paternTo = this.GetPatternByNameTo(raw[2]);
			if (paternTo == null) {
				if (dev) console.log("Cannot load pattern '"+raw[2]+"'");
				return null;
			}
			let item =new ItemVerb();
			item.From=raw[0];
			item.To=raw[0];
			item.PatternFrom=paternFrom;
			item.PatternTo=paternTo;
			return item;
		}
		if (raw.length==2) { 
			let paternFrom = this.GetPatternByNameFrom(raw[0]);
			if (paternFrom == null) {
				if (dev) console.log("Cannot load pattern '"+raw[0]+"'");
				return null;
			}
	
			let paternTo = this.GetPatternByNameTo(raw[1]);
			if (paternTo == null) {
				if (dev) console.log("Cannot load pattern '"+raw[1]+"'");
				return null;
			}
			let item =new ItemVerb();
			item.From="";
			item.To="";
			item.PatternFrom=paternFrom;
			item.PatternTo=paternTo;
			return item;
		}
		return null;
	}

	ForeachArr(fromArr, toArr, fromIndex, toIndex, num, name, match){
		if (fromArr===undefined) return;
		if (toArr===undefined) return;
				
		for (let i=fromIndex; i<toIndex; i++) {
			let shape=fromArr[i];

			// Multiple choises in source array
			if (Array.isArray(shape)) {
				for (const s of shape) {
					if (s=='?') continue;
					let shape=this.From+s;
					
					if (shape==match) {
						if (Array.isArray(toArr[i])) {
							for (const e of toArr[i]) {
								if (e=='?') continue;
								this.ret.push([this.To+e, num, 1+i-fromIndex, name]);
							}
						} else {
							if (toArr[i]=='?') continue;
							this.ret.push([this.To+toArr[i], num, 1+i-fromIndex, name]);
							break;
						}
					}
				}
			} else {
				if (fromArr[i]=="-") continue;
				if (fromArr[i]=='?') continue;

				let shape=this.From+fromArr[i];
					
				if (shape==match) {
					if (Array.isArray(toArr[i])) {
						for (const e of toArr[i]) {
							if (e=='?') continue;
							this.ret.push([this.To+e, num, 1+i-fromIndex, name]);
						}
					} else {
						if (toArr[i]=='?') continue;
						this.ret.push([this.To+toArr[i], num, 1+i-fromIndex, name]);
						break;
					}
				}
			}
		}
	}
	
	IsStringThisWord(str) {
		if (!str.startsWith(this.From)) return null;
		this.ret=[];

		this.ForeachArr(this.PatternFrom.Continous, this.PatternTo.Continous, 0, 3, 1, "Continous", str);
		this.ForeachArr(this.PatternFrom.Continous, this.PatternTo.Continous, 3, 6, 2, "Continous", str);

		this.ForeachArr(this.PatternFrom.Future,this.PatternTo.Future,  0, 3, 1, "Future", str);
		this.ForeachArr(this.PatternFrom.Future, this.PatternTo.Future, 3, 6, 2," Future", str);
		
		this.ForeachArr(this.PatternFrom.PastActive, this.PatternTo.PastActive, 0, 4, 1, "PastActive", str);
		this.ForeachArr(this.PatternFrom.PastActive, this.PatternTo.PastActive, 4, 8, 2, "PastActive", str);
		
		this.ForeachArr(this.PatternFrom.PastPassive, this.PatternTo.PastPassive, 0, 4, 1, "PastPassive", str);
		this.ForeachArr(this.PatternFrom.PastPassive, this.PatternTo.PastPassive, 4, 8, 2, "PastPassive", str);

		// Return all possible falls with numbers
		// [[tvar, číslo, osoba], rod]
	
		{	
			if (this.From+this.PatternFrom.Infinitive==str) {
				if (this.PatternTo.Infinitive!='?') {
				this.ret.push([this.To+this.PatternTo.Infinitive, -1, -1, "Infinitive"]);
				}
			}
		}
/*
		if (this.PatternTo.Continous!==undefined && this.PatternFrom.Continous!==undefined) {
			for (let i=0; i<3; i++) {
				let shape=this.PatternFrom.Continous[i];
				if (Array.isArray(shape)){


				}
				for (let s of shape) {
					let shape=this.From+this.PatternFrom.Continous[i];
					//if (shape=="-") continue;

					if (shape==str) {
						ret.push([this.To+this.PatternTo.Continous[i], 1, i+1, "Continous"]);
						break;
					}

				
				}
			}
			for (let i=3; i<6; i++) {
				let shape=this.From+this.PatternFrom.Continous[i];
				//if (shape=="-") continue;
				if (shape==str) {
					ret.push([this.To+this.PatternTo.Continous[i], 2, i-3+1, "Continous"]);
					break;
				}
			}
		}

		if (this.PatternTo.Future!==undefined && this.PatternFrom.Future!==undefined) {
			for (let i=0; i<3; i++) {
				let shape=this.From+this.PatternFrom.Future[i];
				//if (shape=="-") continue;

				if (shape==str) {
					ret.push([this.To+this.PatternTo.Future[i], 1, i+1, "Future"]);
					break;
				}
			}
			for (let i=3; i<6; i++) {
				let shape=this.From+this.PatternFrom.Future[i];
				//if (shape=="-") continue;

				if (shape==str) {
					ret.push([this.To+this.PatternTo.Future[i], 2, i-3+1, "Future"]);
					break;
				}
			}
		}

		if (this.PatternTo.Imperative!==undefined && this.PatternFrom.Imperative!==undefined) {
			for (let i=0; i<1; i++) {
				let shape=this.From+this.PatternFrom.Imperative[i];
				//if (shape=="-") continue;

				if (shape==str) {
					ret.push([this.To+this.PatternTo.Imperative[i], 1, i+1, "Imperative"]);
					break;
				}
			}
			for (let i=1; i<3; i++) {
				let shape=this.From+this.PatternFrom.Imperative[i];
				//if (shape=="-") continue;
				if (shape==str) {
					ret.push([this.To+this.PatternTo.Imperative[i], 2, i, "Imperative"]);
					break;
				}
			}
		}

		if (this.PatternTo.PastPassive!==undefined && this.PatternFrom.PastPassive!=undefined) {
			for (let i=0; i<4; i++) {
				if (Array.isArray(this.PatternFrom.PastPassive[i])) {
					for (const z of this.PatternFrom.PastPassive[i]) {
						if (this.From+z==str) {
							ret.push([this.To+this.PatternTo.PastPassive[i], 1, i+1, "PastPassive"]);
							break;
						}
					}
				} else {
					let shape=this.From+this.PatternFrom.PastPassive[i];
					//if (shape=="-") continue;

					if (shape==str) {
						ret.push([this.To+this.PatternTo.PastPassive[i], 1, i+1, "PastPassive"]);
						break;
					}
				}
			}
			for (let i=4; i<8; i++) {
				if (Array.isArray(this.PatternFrom.PastPassive[i])) {
					for (const z of this.PatternFrom.PastPassive[i]) {
						if (this.From+z==str) {
							ret.push([this.To+this.PatternTo.PastPassive[i], 2, i+1-4, "PastPassive"]);
							break;
						}
					}
				} else {
					let shape=this.From+this.PatternFrom.PastPassive[i];
					//if (shape=="-") continue;
					if (shape==str) {
						ret.push([this.To+this.PatternTo.PastPassive[i], 2, i-4+1, "PastPassive"]);
						break;
					}
				}
			}
		}

		if (this.PatternTo.PastActive!==undefined) {
			for (let i=0; i<4; i++) {
				let shape=this.PatternFrom.PastActive[i];
				//if (shape=="-") continue;

				if (this.From+shape==str) {
					ret.push([this.To+this.PatternTo.PastActive[i], 1, i+1, "PastActive"]);
					break;
				}
			}
			for (let i=4; i<8; i++) {
				let shape=this.PatternFrom.PastActive[i];
				//if (shape=="-") continue;

				if (this.From+shape==str) {
					ret.push([this.To+this.PatternTo.PastActive[i], 2, i-3+1, "PastActive"]);
					break;
				}
			}
		}*/
		
		if (this.PatternTo.Auxiliary!==undefined && this.PatternFrom.Auxiliary!==undefined) {
			for (let i=0; i<3; i++) {
				let shape=this.PatternFrom.Auxiliary[i];
				//if (shape=="-") continue;

				if (this.From+shape==str) {
					this.ret.push([this.To+this.PatternTo.Auxiliary[i], 1, i+1, "Auxiliary"]);
					break;
				}
			}
			for (let i=3; i<6; i++) {
				let shape=this.PatternFrom.Auxiliary[i];
				//if (shape=="-") continue;

				if (this.From+shape==str) {
					this.ret.push([this.To+this.PatternTo.Auxiliary[i], 2, i-3+1, "Auxiliary"]);
					break;
				}
			}
		}
		if (this.ret.length==0) return null; else return this.ret;

	}

	static GetPatternByNameFrom(name) {
		for (const p of this.pattensFrom) {
			if (p.Name==name) return p;
		}
	}

	static GetPatternByNameTo(name) {
		for (const p of this.pattensTo) {
			if (p.Name==name) return p;
		}
	}

	GetDicForm(name) {		
		return [this.From+this.PatternFrom[0], this.To+this.PatternTo[0], "", name];
	}
} 

class Replace {
	constructor() {
		this.Input=[];
		this.Output=[];
	}
}

class LanguageTr {
	constructor() {
		this.Name="";
	//	this.myVocab=[];
	//	this.Words=[];
		//this.SameWords=[];
		this.Sentences=[];
		this.Phrases=[];
		this.state="instanced";
		this.html=true;
		this.SelectReplace=[];
		this.SentencePatterns=[];
		this.SentencePatternParts=[];
		this.SentenceParts=[];
	//	dev=false;
		this.SimpleWords=[];
		this.PatternNounsFrom=[];
		this.PatternNounsTo=[];
		this.PatternAdjectivesFrom=[];
		this.PatternAdjectivesTo=[];
		this.PatternPronounsFrom=[];
		this.PatternPronounsTo=[];
		this.PatternNumbersFrom=[];
		this.PatternNumbersTo=[];
		this.PatternVerbsFrom=[];
		this.PatternVerbsTo=[];
		this.Nouns=[];
		this.Pronouns=[];
		this.Verbs=[];
		this.Adjectives=[];
		this.Numbers=[];
		this.Prepositions=[];
		this.Interjections=[];
		this.Particles=[];
		this.Adverbs=[];
		this.Conjunctions=[];
		this.option=undefined;
		this.ReplaceS=[];
		this.ReplaceG=[];
		this.ReplaceE=[];

		this.MakeBold="\x1b[1m";
		this.MakeUnderline="\x1b[4m";
		this.MakeGreen="\x1b[32m";
		this.MakeBlue="\x1b[34m";
		this.MakeCyan="\x1b[36m";

		this.qualityTrTotalTranslatedWell=0.0;
		this.qualityTrTotalTranslated=0.0;

		
		this.locationX=NaN;
		this.locationY=NaN;
		this.gpsX=NaN;
		this.gpsY=NaN;
		this.Quality=0;
		this.Author="";
		this.LastDateEdit="";
		this.Comment="";
		this.baseLangName=null;
	}
/*
	GetVocabulary() {
		this.state="downloading";
	//	dev=dev;
		if (dev) console.log("INFO| Starting Downloading '"+this.Name+".trw'");
		let request=new XMLHttpRequest();
		request.timeout=4000;
		request.open('GET', "DIC/"+this.Name+".trw", true);
		request.send();
		let self=this;
		request.onerror=function() {
			if (dev) console.log("ERROR| Cannot downloaded '"+self.Name+".trw'");
			this.state="cannot download";
		};
		let x=this;
		request.onreadystatechange=function() {
			this.state="download status "+request.status;
			if(request.readyState===4) {
				this.state="download status "+request.status;
				if(request.status===200) {
					this.state="downloaded";
					if(dev) console.log("INFO| Downloaded '"+self.name+".trw'");

					let text=request.responseText;
						
					let lines=text.split('\r\n');
					if(lines.length<5&&dev) {
						if(dev) console.log("WARLING| Downloaded '"+self.name+".trw' seems too small");
						enabletranslate=false;
						ReportDownloadedLanguage();
						return;
					}
					// document.getElementById('slovnik').innerText = lines.length;
					x.Load(lines);
					ReportDownloadedLanguage();
				} else {
					if(request.status==0) ShowError("<b>Jejda, něco se pokazilo.</b><br>Nelze stáhnout seznam slovíček překladu...'"+self.name+"'<br>");
					else ShowError("<b>Jejda, něco se pokazilo.</b><br>Nelze stáhnout seznam slovíček překladu<br>Chyba '"+request.status+"..."+self.name+"'<br>");
					DisableLangTranslate(self.name);
					ReportDownloadedLanguage();
					this.state="download errored";
					return;
				}
			}
		};
	}*/

	Load(lines) {
		this.state="loading";
		enabletranslate=true;
		if (dev) console.log("INFO| Parsing "+this.Name);
		// Head
		let i=0;
		for(; i<lines.length; i++) {
			let line=lines[i];

			if (line=="-") break;
			let subtype=line.substring(0, 1);
			switch(subtype) {
				// Comment info
				case "i":
					this.Info = line.substring(1).replaceAll('\\n',"<br>");
					break;

				case "a":
					this.Author = line.substring(1);
					break;

				case "d":
					this.LastDateEdit = line.substring(1);
					break;

				case "f":
					this.From= line.substring(1);
					break;

				case "t":
					this.Name = line.substring(1);
					break;

				case "e":
					//this.BuildSelect(line.substring(1));
					break;

				case "c":
					{
						let stri=line.substring(1);
						if (stri instanceof  String || typeof myVar === 'string') {
							let l=stri.replaceAll('\\n',"\n").replaceAll("->","➔").split('\n');
							let text="";
							let ul=false;
							for (let i of l){
								if (i.startsWith("#")) {
									if (ul) {text+='</ul>';ul=false;}
									text+='<p style="display:inline-block" class="settingHeader">'+i.substring(1)+'</p>';
								}else if (i.startsWith("-")) {
									if (!ul) text+='<ul>';
									text+='<li>'+i.substring(1)+'</li>';
									ul=true;
								}else if (i==""){
									if (ul) {text+='</ul>';ul=false;}
									text+='<br>';
								}else {
									if (ul) {text+='</ul>';ul=false;}
									text+='<p>'+i+'</p>';
								}

							}
							this.Comment = text;
						}
					}
					break;
					
				case "g":
					{
						let GPS=line.substring(1);
						//console.log(GPS);
						if (GPS.includes(',')) {
							let rawPos=GPS.split(',');
							this.gpsX=parseFloat(rawPos[0]);
							this.gpsY=parseFloat(rawPos[1]);

							//let originX=14.6136976, originY=50.4098883,scX=4.07, scY=1.8483;
						//	this.locationX=(((this.gpsY-originX)/scX)*170*1.21-20.92)*3.8;
							//this.locationY=((-(this.gpsX-originY)/scY)*150*1.0367+3.4)*3.8;
							let xZ=686/3.6173147-5, xM=14.87480,
							    yZ=415/1.4573454, yM=48.41226;
							this.locationX=(this.gpsY-xM)*xZ-15;
							this.locationY=566-(this.gpsX-yM)*yZ+30;
						//	if (this.Name=="Nymburk" || this.Name=="Rybnik" || this.Name=="Handlova")console.log(this.Name, this.gpsY, this.locationY);
						}
					}
					break;
					
				case "q":
					this.Quality=parseFloat(line.substring(1));
					break;
					
				case "l":
					this.lang=line.substring(1);
					break;

				case "o":
					this.Category=line.substring(1).split('>');
					break;

				case "z":
					this.baseLangName=line.substring(1);
					break;
			}
		}
			
		// SentencePattern
		for (i++; i<lines.length; i++) {
			let line=lines[i];
			if (line == "-") break;

			let item=ItemSentencePattern.Load(line);
			if (item !== null && item !== undefined) this.SentencePatterns.push(item);
			else if (dev) console.log("⚠️ Cannot load 'SentencePattern' item at line "+i+". ", line);
		}
		if (dev) console.log("🔣 Loaded SentencePatterns", this.SentencePatterns);
		
		// SentencePartPattern
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line=="-") break;

			let item=ItemSentencePatternPart.Load(line);
            if (item !== null && item !== undefined) this.SentencePatternParts.push(item);
			else if (dev) console.log("⚠️ Cannot load 'SentencePartPattern' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded SentencePartPatterns", this.SentencePartPatterns);

	
        // Sentences
        for (i++; i<lines.length; i++) {
			let line=lines[i];
			if (line == "-") break;

			let item=ItemSentence.Load(line);
            if (item !== null && item !== undefined) this.Sentences.push(item);
			else if (dev) console.log("⚠️ Cannot load 'Sentence' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded Sentences", this.Sentences);

		 // SentencesParts
		 for (i++; i<lines.length; i++) {
			let line=lines[i];
			if (line == "-") break;

			let item=ItemSentencePart.Load(line);
            if (item !== null && item !== undefined) this.SentenceParts.push(item);
			else if (dev) console.log("⚠️ Cannot load 'SentencePart' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded SentenceParts", this.SentenceParts);


		// Phrase
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line=="-") break;

			let item=ItemPhrase.Load(line);
            if (item !== null && item !== undefined) this.Phrases.push(item);
			else if (dev) console.log("⚠️ Cannot load 'Phrase' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded Phrases", this.Phrases);

        // SimpleWords
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line == "-") break;

			let item=ItemSimpleWord.Load(line);
            if (item !== null && item !== undefined) this.SimpleWords.push(item);
			else if (dev) console.log("⚠️ Cannot load 'SimpleWord' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded SimpleWords", this.SimpleWords);


		// ReplaceS
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line=="-") break;

			let item=ItemReplaceS.Load(line);
            if (item !== null && item !== undefined) this.ReplaceS.push(item);
			else if (dev) console.log("⚠️ Cannot load 'ReplaceS' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded ReplaceSs", this.ReplaceS);
		this.ReplaceS.sort((a, b) => (a.input.length < b.input.length) ? 1 : -1);

		// ReplaceG
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line=="-") break;

			let item=ItemReplaceG.Load(line);
            if (item !== null && item !== undefined) this.ReplaceG.push(item);
			else if (dev) console.log("⚠️ Cannot load 'ReplaceG' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded ReplaceGs", this.ReplaceG);
		this.ReplaceG.sort((a, b) => (a.input.length < b.input.length) ? 1 : -1);

		// ReplaceE
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line=="-") break;

			let item=ItemReplaceE.Load(line);
            if (item !== null && item !== undefined) this.ReplaceE.push(item);
			else if (dev) console.log("⚠️ Cannot load 'ReplaceE' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded ReplaceEs", this.ReplaceE);


        // PatternNounFrom
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line == "-") break;

			let item = ItemPatternNoun.Load(line);
			if (item !== null && item !== undefined) this.PatternNounsFrom.push(item);
			else if (dev) console.log("⚠️ Cannot load 'PatternNoun' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded PatternNounsFrom", this.PatternNounsFrom);

		// PatternNounTo
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line == "-") break;

			let item = ItemPatternNoun.Load(line);
			if (item !== null && item !== undefined) this.PatternNounsTo.push(item);
			else if (dev) console.log("⚠️ Cannot load 'PatternNoun' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded PatternNounsTo", this.PatternNounsTo);

        // Noun
		ItemNoun.pattensFrom=this.PatternNounsFrom;
		ItemNoun.pattensTo=this.PatternNounsTo;
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line=="-") break;

			let item=ItemNoun.Load(line);
            if (item !== null && item !== undefined) this.Nouns.push(item);
			else if (dev) console.log("⚠️ Cannot load 'Noun' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded Nouns", this.Nouns);

        // PatternAdjectivesFrom
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line=="-") break;

			let item=ItemPatternAdjective.Load(line);
			if (item !== null && item !== undefined) this.PatternAdjectivesFrom.push(item);
			else if (dev) console.log("⚠️ Cannot load 'PatternAdjective' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded PatternAdjectivesFrom", this.PatternAdjectivesFrom);

        // PatternAdjectivesTo
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line=="-") break;

			let item=ItemPatternAdjective.Load(line);
			if (item !== null && item !== undefined) this.PatternAdjectivesTo.push(item);
			else if (dev) console.log("⚠️ Cannot load 'PatternAdjective' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded PatternAdjectivesTo", this.PatternAdjectivesTo);

        // Adjectives
		ItemAdjective.pattensFrom=this.PatternAdjectivesFrom;
		ItemAdjective.pattensTo=this.PatternAdjectivesTo;
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line=="-") break;

			let item=ItemAdjective.Load(line);
            if (item !== null && item !== undefined) this.Adjectives.push(item);
			else if (dev) console.log("⚠️ Cannot load 'Adjective' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded Adjectives", this.Adjectives);

        // PatternPronounsFrom
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line=="-") break;

			let item=ItemPatternPronoun.Load(line);
            if (item !== null && item !== undefined) this.PatternPronounsFrom.push(item);
			else if (dev) console.log("⚠️ Cannot load 'PatternPronoun' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded PatternPronounsFrom", this.PatternPronounsFrom);
		
		// PatternPronounsTo
		for (i++; i<lines.length; i++) {
			let line=lines[i];
			if (line=="-") break;

			let item=ItemPatternPronoun.Load(line);
			if (item !== null && item !== undefined) this.PatternPronounsTo.push(item);
			else if (dev) console.log("⚠️ Cannot load 'PatternPronoun' item at line "+i, line);
		}
		if (dev) console.log("🔣 Loaded PatternPronounsTo", this.PatternPronounsTo);

        // Pronouns
		ItemPronoun.pattensFrom=this.PatternPronounsFrom;
		ItemPronoun.pattensTo=this.PatternPronounsTo;
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line=="-") break;

			let item=ItemPronoun.Load(line);
            if (item !== null && item !== undefined) this.Pronouns.push(item);
			else if (dev) console.log("⚠️ Cannot load 'Pronoun' item at line "+i+". Data: ", line);
        }
		if (dev) console.log("🔣 Loaded Pronouns", this.Pronouns);

        // PatternNumbersFrom
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line=="-") break;

			let item=ItemPatternNumber.Load(line);
            if (item !== null && item !== undefined) this.PatternNumbersFrom.push(item);
			else if (dev) console.log("⚠️ Cannot load 'PatternNumber' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded PatternNumbersFrom", this.PatternNumbersFrom);

		// PatternNumbersTo
		for (i++; i<lines.length; i++) {
			let line=lines[i];
			if (line=="-") break;

			let item=ItemPatternNumber.Load(line);
			if (item !== null && item !== undefined) this.PatternNumbersTo.push(item);
			else if (dev) console.log("⚠️ Cannot load 'PatternNumber' item at line "+i, line);
		}
		if (dev) console.log("🔣 Loaded PatternNumbersTo", this.PatternNumbersTo);

        // Numbers
		ItemNumber.pattensFrom=this.PatternNumbersFrom;
		ItemNumber.pattensTo=this.PatternNumbersTo;
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line=="-") break;
			
			let item=ItemNumber.Load(line);
            if (item !== null && item !== undefined) this.Numbers.push(item);
			else if (dev) console.log("⚠️ Cannot load 'Number' item at line "+i+", data: ", line);
        }
		if (dev) console.log("🔣 Loaded Numbers", this.Numbers);

        // PatternVerbsFrom
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line=="-") break;

			let item=ItemPatternVerb.Load(line);
            if (item !== null && item !== undefined) this.PatternVerbsFrom.push(item);
			else if (dev) console.log("⚠️ Cannot load 'PatternVerb' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded PatternVerbsFrom", this.PatternVerbsFrom);
    
		// PatternVerbsTo
		for (i++; i<lines.length; i++) {
			let line=lines[i];
			if (line=="-") break;

			let item=ItemPatternVerb.Load(line);
			if (item !== null && item !== undefined) this.PatternVerbsTo.push(item);
			else if (dev) console.log("⚠️ Cannot load 'PatternVerb' item at line "+i, line);
		}
		if (dev) console.log("🔣 Loaded PatternVerbsTo", this.PatternVerbsTo);

        // Verb
		ItemVerb.pattensFrom=this.PatternVerbsFrom;
		ItemVerb.pattensTo=this.PatternVerbsTo;
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line=="-") break;

			let item=ItemVerb.Load(line);
            if (item !== null && item !== undefined) this.Verbs.push(item);
			else if (dev) console.log("⚠️ Cannot load 'Verb' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded Verbs", this.Verbs);

		// Adverb
		for (i++; i<lines.length; i++) {
			let line=lines[i];
			if (line=="-") break;

			let item=ItemSimpleWord.Load(line);
			if (item !== null && item !== undefined) this.Adverbs.push(item);
			else if (dev) console.log("⚠️ Cannot load 'Adverb' item at line "+i, line);
		}
		if (dev) console.log("🔣 Loaded Adverbs", this.Adverbs);
		
		// Preposition
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line=="-") break;

			let item=ItemPreposition.Load(line);
            if (item !== null && item !== undefined) this.Prepositions.push(item);
			else if (dev) console.log("⚠️ Cannot load 'Preposition' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded Prepositionss", this.Prepositions);

		// Conjunction
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line=="-") break;

			let item=ItemSimpleWord.Load(line);
            if (item !== null && item !== undefined) this.Conjunctions.push(item);
			else if (dev) console.log("⚠️ annot load 'Conjunction' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded Conjunctions", this.Conjunctions);

		// Particle
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line=="-") break;

			let item=ItemSimpleWord.Load(line);
            if (item !== null && item !== undefined) this.Particles.push(item);
			else if (dev) console.log("⚠️ Cannot load 'Particle' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded Particles", this.Particles);

		// Interjection
        for (i++; i<lines.length; i++) {
            let line=lines[i];
            if (line=="-") break;

			let item=ItemSimpleWord.Load(line);
            if (item !== null && item !== undefined) this.Interjections.push(item);
			else if (dev) console.log("⚠️ Cannot load 'Interjection' item at line "+i, line);
        }
		if (dev) console.log("🔣 Loaded Interjections", this.Interjections);

		
		this.ReplaceE.sort((a, b) => (a.input.length < b.input.length) ? 1 : -1);

		this.state="loaded";
	}

	GetDic(input) {
		function IsWordIncluded(w) {
			if (Array.isArray(w)) {
				for (let c of w){
					if (c.includes(input)) {
						return true;
					}
				}
				return false;
			}else{
				return w.startsWith(input);
			}
		}
		
		function IsWordComIncluded(w) {
			if (w.PatternFrom==undefined) return false;
			let ww=w.From+w.PatternFrom[0];
			return ww.startsWith(input);
		}

		let out=[];
		let total=0;
		for (let w of this.Phrases) {
			if (IsWordIncluded(w.input)) {
				let g=w.GetDicForm("fráze");
				if (g!=null) out.push(g);
				total++;
			}
		}	
		for (let w of this.Adverbs) {
			if (IsWordIncluded(w.input)) {
				let g=w.GetDicForm(" (přís.)");
				if (g!=null) out.push(g);
				total++;
			}
		}
		for (let w of this.Particles) {
			if (IsWordIncluded(w.input)) {
				let g=w.GetDicForm(" (část.)");
				if (g!=null) out.push(g);
				total++;
			}
		}
		for (let w of this.Conjunctions) {
			if (IsWordIncluded(w.input)) {
				let g=w.GetDicForm(" (spoj.)");
				if (g!=null) out.push(g);
				total++;
			}
		}
		for (let w of this.Interjections) {
			if (IsWordIncluded(w.input)) {
				let g=w.GetDicForm(" (cito.)");
				if (g!=null) out.push(g);
				total++;
			}
		}	

		for (let w of this.SimpleWords) {
			if (IsWordIncluded(w.input)) {
				let g=w.GetDicForm("");
				if (g!=null) out.push(g);
				total++;
			}
		}

		for (let w of this.Sentences) {
			if (IsWordIncluded(w.input)) {
				let g=w.GetDicForm("věta");
				if (g!=null) out.push(g);
				total++;
			}
		}
		
		for (let w of this.SentenceParts) {
			if (IsWordIncluded(w.input)) {
				let g=w.GetDicForm("část věty");
				if (g!=null) out.push(g);
				total++;
			}
		}
		
		for (let w of this.Nouns) {
			if (IsWordComIncluded(w)) {
				let g=w.GetDicForm("(pods.)");
				if (g!=null) out.push(g);
				total++;
			}
		}
		
		for (let w of this.Pronouns) {
			if (IsWordComIncluded(w)) {
				let g=w.GetDicForm("(zájm.)");
				if (g!=null) out.push(g);
				total++;
			}
		}
		for (let w of this.Verbs) {
			if (IsWordComIncluded(w)) {
				let g=w.GetDicForm("(slov.)");
				if (g!=null) out.push(g);
				total++;
			}
		}
	
		for (let w of this.Adjectives) {
			if (IsWordComIncluded(w)) {
				let g=w.GetDicForm("(příd.)");
				if (g!=null) out.push(g);
				total++;
			}
		}
		
		for (let w of this.Numbers) {
			if (IsWordComIncluded(w)) {
				let g=w.GetDicForm("(čísl.)");
				if (g!=null) out.push(g);
				total++;
			}
		}
		
		for (let w of this.Prepositions) {
			if (IsWordIncluded(w.input)) {
				let g=w.GetDicForm("(před.)");
				if (g!=null) out.push(g);
				total++;
			}
		}		
		
		let display="";
		if (!dev){
			for (let i=0; i<out.length; i++) {
				if (out[i].join("").includes("undefined")) {
					out.splice(i, 1);
				}
			}
		}
		if (out=="") 		display= "<p style='font-style: italic'>Nenalezen žádný záznam.</p>";
		else if (total==1)	display= "<p style='font-style: italic'>Nalezen "+total+" záznam.</p>";
		else if (total<5)	display= "<p style='font-style: italic'>Nalezeny "+total+" záznamy.</p>";
		else 				display= "<p style='font-style: italic'>Nalezeno celkem "+total+" záznamů.</p>";
		out=out.sort((a, b) => {
			if (a[0] instanceof String) return a[0].localeCompare(b[0]);
			return  false;
		});

		let zkr=false;
		if (out.length>50){ out.splice(50,out.length-50);zkr=true;}

		if (out.length!=0) {
			for (let z of out) {
				if (z[2]=="")display+="<p>"+z[0]+" → "+z[1]+"  <i>"+z[3]+"</i></p>";
				else display+="<p>"+z[0]+" → "+z[1]+"; "+z[2]+"  <i>"+z[3]+"</i></p>";
			}
		}
		if (zkr) display+="<p style='font-style: italic'>Seznam zkrácen.</p>";
		return display;
	}

	Translate(input, html) {
		this.qualityTrTotalTranslatedWell=0;
		this.qualityTrTotalTranslated=0;
		this.html=html;

		PrepareReplaceRules();

		if (dev) console.log("📝 Translating "+this.Name+"...");

		let output=document.createElement("div");
		let stringOutput="";

		let sentences=this.SplitSentences(input, ".!?");
		
		for (let i=0; i<sentences.length; i++) {
			let currentSentenceS=sentences[i];
			let currentSentence=currentSentenceS/*[1]*/;
			if (dev) console.log("📘 \x1b[1m\x1b[34mCurrent Sentence: ", currentSentence);
			// Add . ? !
		//	if (!currentSentenceS[0]) {
		//		this.AddText(currentSentence, output, "symbol");
		//		continue;
		//	}

			// In cases like ... or !!! or !?
			if (currentSentence=="") continue;

			// Simple replece full sentences
			let m=this.matchSentence(currentSentence);
			if (m !== null) {
				this.AddText(m.output,output,"sentence");
				continue;
			}

			// Sentence pattern
			let patternDone=this.SolveSentencePattern(currentSentence);
			if (patternDone!= null) {
				
				let space=document.createTextNode(" ");
				output.appendChild(space);

				output.appendChild(patternDone);
				continue;
			}

			if (dev) console.log("Sentence pattern not found",currentSentence);

			// Words
			let unknownPattern;
	//		let sent=currentSentence.substring(currentSentence.length-1);

			let words=this.MultipleSplit(currentSentence, "  ,-:;'\t_.!?„“\n");
			
			let BuildingSentence=[];
			/*
			Create sencence without relationships
			arr = ["string to draw", ["noun/adjective/symbol", other options, ..]]
			*/
			for (let w=0; w<words.length; w++) {
				let currentWord=words[w];
				let Zword=currentWord[1];
				let word=Zword.toLowerCase();

				// Phrases?
				

				// separator
				if (!currentWord[0]) {
					let repair=this.normalizeSymbols(word);
					BuildingSentence.push(["Symbol", repair, Zword]);
					continue;
				}

				// Phases apply
				let phr=this.ApplyPhrases(words, w);
				if (phr!=null){
					BuildingSentence.push(phr);
					console.log(phr);
					continue;
				}

				// foreach words
			/*	{
					let n=this.searchWordPhrase(word);
					if (n!=null) {
						BuildingSentence.push(["Phrase", n, Zword]);
						continue;
					}
				}*/
				{
					let n=this.searchWordNoun(word);
					if (n!=null) {
						// n=[[tvar, číslo, pád], rod];
						BuildingSentence.push(["Noun", n, Zword]);
						continue;
					}
				}
				{
					let n=this.searchWordAdjective(word);
					if (n!=null) {
						// n=[[tvar, číslo, pád], rod];
						BuildingSentence.push(["Adjective", n, Zword]);
						continue;
					}
				}
				{
					let n=this.searchWordPronoun(word);
					if (n!=null) {
						// n=[[tvar, číslo, pád], rod];
						BuildingSentence.push(["Pronoun", n, Zword]);
						continue;
					}
				}
				{
					let n=this.searchWordNumber(word);
					if (n!=null) {
						// n=[[tvar, číslo, pád], rod];
						BuildingSentence.push(["Number", n, Zword]);
						continue;
					}
				}
				{
					let n=this.searchWordVerb(word);
					if (n!=null) {
						// n=[[tvar, číslo, pád], rod];
						BuildingSentence.push(["Verb", n, Zword]);
						continue;
					}
				}
				{
					let n=this.searchWordAdverb(word);
					if (n!=null) {
						
						BuildingSentence.push(["Adverb", n, Zword]);
						continue;
					}
				}
				{
					let n=this.searchWordPreposition(word);
					if (n!=null) {
						// n=[out, falls];
						let out=n[0];
						let falls=n[1];
						BuildingSentence.push(["Preposition", n, Zword]);
						continue;
					}
				}
				{
					let n=this.searchWordParticle(word);
					if (n!=null) {
						BuildingSentence.push(["Particle", n, Zword]);
						continue;
					}
				}
				{
					let n=this.searchWordConjunction(word);
					if (n!=null) {
						BuildingSentence.push(["Conjunction", n, Zword]);
						continue;
					}
				}
				{
					let n=this.searchWordInterjection(word);
					if (n!=null) {
						BuildingSentence.push(["Interjection", n, Zword]);
						continue;
					}
				}
				{
					let n=this.searchSimpleWord(word);
					if (n!=null) {
						BuildingSentence.push(["SimpleWord", n, Zword]);
						continue;
					}
				}
				{
					// If is number
					if (isNumber(word)) {// === +word
						BuildingSentence.push(["NumberLetters", word, Zword]);
						continue;
					}
				}

				// Add unknown word
				let TryReplaces=this.ReplaceWord(word);
				BuildingSentence.push(["Unknown", TryReplaces, Zword]);
				continue;
			}

			if (dev) console.log("Sencence.. Before relationship.. ", BuildingSentence);
			
			// Create relationships
			// Po předložce nastav u podtatného nebo přídavného pád a rod
			let startIndex=-1, endIndex=-1;
			for (let w=0; w<BuildingSentence.length; w++) {
				let word=BuildingSentence[w];
				if (word[0]=="Preposition") {
					if (startIndex==-1) {
						startIndex=w;
					} else {
						endIndex=w-1;
					//	MakeprepositionAdjectiveNounRelationShip(BuildingSentence, startIndex, endIndex);
						startIndex=-1;
						endIndex-1;
					}
				} else if (word[0]=="Noun" || word[0]=="Adjective" || word[0]=="Number") {
				} else {
					if (startIndex!=-1) {
						endIndex=w-1;
					//	MakeprepositionAdjectiveNounRelationShip(BuildingSentence, startIndex, endIndex);
						startIndex=-1;
						endIndex-1;
					}
				}
			}
			
			if (dev) console.log("Sencence.. With relationship.. ", BuildingSentence);

			// Print
			for (let w=0; w<BuildingSentence.length; w++) {
				let word=BuildingSentence[w];
				let type=word[0];
				let string=word[1];
				let original=word[2];

				let printableString;
				if (type=="Noun") {
					printableString=string[0]
				}else if (type=="Adjective") printableString=string;
				else if (type=="Pronoun") printableString=string[0];
				else if (type=="Number") printableString=string[0];
				else if (type=="Verb") printableString=string;
				else if (type=="Adverb") printableString=string.output;
				else if (type=="Preposition") printableString=string[0];
				else if (type=="Conjunction") printableString=string.output;
				else if (type=="Phrase") printableString=string/*.output*/;
				else if (type=="Particle") printableString=string.output;
				else if (type=="Interjection") printableString=string.output;
				else if (type=="Symbol") printableString=string;
				else if (type=="Unknown") {
					if (Array.isArray(string))printableString=string[0];
					else printableString=string;
				}
				else if (type=="SimpleWord") printableString=string.output;
				else if (type=="NumberLetters") printableString=string;
				else {
					if (dev) console.log("Unknown", string);
					printableString=string;
				}

			//	if (html) {
					// Write how well translated
					if (type!=="Unknown" && type!=="Symbol") this.qualityTrTotalTranslatedWell++;
					if (type!=="Symbol")this.qualityTrTotalTranslated++;
			//	}
				let resStr=this.PrepareText(printableString);
				let retText;

				// All uppercase
				if (original==original.toUpperCase()) {
					if (Array.isArray(resStr)){
						let arr=[];
						for (let x of resStr) arr.push(x.toUpperCase());
						retText=arr;
					}else{
						retText=resStr.toUpperCase();
					}
				// All uppercase
				} else if (original[0]==original[0].toUpperCase()) {
					if (Array.isArray(resStr)){
						let arr=[];
						for (let x of resStr) {
							if (x.length==0) continue;
							if (x.length==1) {
								arr.push(x[0].toUpperCase());
							} else {
								arr.push(x[0].toUpperCase() + x.substring(1));
							}							
						}
						retText=arr;
					} else {
						if (resStr.length>0) {
							retText=resStr[0].toUpperCase() + resStr.substring(1);
						} else retText=resStr;
					}
				}else{
					retText=resStr;
				}

				stringOutput+=this.AddText(retText, output, type);

				
			//	this.AddText(word, output, "numberLetters");
			}
			if (this.html) {
				let space=document.createTextNode(" ");
				output.appendChild(space);
			} else stringOutput+=" ";
		}
		if (this.html) {
			let quality=(Math.round((this.qualityTrTotalTranslatedWell/this.qualityTrTotalTranslated)*100)).toString();
			if (quality=="100") quality="99";
			else if (quality.length==1) quality="0"+quality;
			else if (quality=="NaN") quality="?";
			document.getElementById("translateWellLevel").innerText="Q"+quality;
		}
		if (html) return output;
		else return stringOutput;
	}

	sentenceIncludesWord(sentence, word) {

	}

	normalizeSymbols(symbol) {
		// Správné uvozovky
		if (symbol=='"') return '“';
		if (symbol=="'") return '‘';

		// nbsp
		if (symbol==" ") return ' ';

		return symbol;
	}

	matchSentence(input) {
		if (!input.endsWith("!") && !input.endsWith("?") && !input.endsWith(".")) {
			
				for (const s of this.Sentences) {
				//	console.log(s.input.substring(0, s.input.length-1));
					if (s.input.length>2) {
						if (s.input.substring(0, s.input.length-1)==input) return s;
						//if (s.input==input) return s;
					}
				}
				return null;
		
		}
		for (const s of this.Sentences) {
			if (s.input==input) return s;
		}
		return null;
	}
	
	searchWordNoun(input) {
		for (const n of this.Nouns) {
			let z=n.IsStringThisWord(input);
			if (z!==null) return z;
		}
		return null;
	}

	searchWordAdjective(input) {
		for (const n of this.Adjectives) {
			let z=n.IsStringThisWord(input);
			if (z!==null) return z;
		}
		return null;
	}
	
	searchWordNumber(input) {
		for (const n of this.Numbers) {
			let z=n.IsStringThisWord(input);
			if (z!==null) return z;
		}
		return null;
	}

	searchWordPhrase(input) {
		for (const n of this.Phrases) {
			if (n.input==input) return n;
		}
		return null;
	}

	searchWordVerb(input) {
		for (const n of this.Verbs) {
			let z=n.IsStringThisWord(input);
			if (z!==null) return z;
		}
		return null;
	}

	searchWordPronoun(input) {
		for (const n of this.Pronouns) {
			let z=n.IsStringThisWord(input);
			if (z!==null) return z;
		}
		return null;
	}

	searchWordPreposition(input) {
		for (const n of this.Prepositions) {
			let z=n.IsStringThisWord(input);
			if (z!==null) return z;
		}
		return null;
	}
	
	searchSimpleWord(input) {
		for (const n of this.SimpleWords) {
			if (n.input==input) return n;
		}
		return null;
	}

	searchWordInterjection(input) {
		for (const n of this.Interjections) {
			if (n.input==input) return n;
		}
		return null;
	}

	searchWordConjunction(input) {
		for (const n of this.Conjunctions) {
			if (n.input==input) return n;
		}
		return null;
	}

	searchWordParticle(input) {
		for (const n of this.Particles) {
			if (n.input==input) return n;
		}
		return null;
	}

	searchWordAdverb(input) {
		for (const n of this.Adverbs) {

			//console.log("Try", n.input,input);
			//if (n.input==input) return n;
			if (Array.isArray(n.input)){
				for (const m of n.input) {
					if (m==input) return n;
				}
			} else {
				if (n.input==input) return n;
			}
		}
		return null;
	}

	SolveSentencePattern(input) {
		let opInp;

		for (const pattern of this.SentencePatterns) {
			//console.log("Try pattern: |"+input+"|", pattern);
			let isNot=false;
			// Je to on?			
			// zapadá? ["Dal jsi to ", ["vedoucímu", rules, 0], ", ten ", ["spis", rules, 1], "?"];
			let prevWasRule;
			let rawRules=[];
			opInp=input.slice();

			for (let part of pattern.input) {
			//	console.log("pattern sentence", part, typeof part);
				if (typeof part === 'string') {	
				//	console.log("Detected string", part);
					/*if (prevWasRule){
						
						prevWasRule=false;
						let indexOfNext=opInp.indexOf(part);
						if (indexOfNext==-1) {
							// End
							isNot=true;
							break;
						}
						console.log("X1_2");
						if (opInp.includes(' ')){
							let indexOfNextSpace=opInp.indexOf(" ");
							console.log("X1_3");
							// After rule unknown word
							if (indexOfNextSpace+1!=indexOfNext ) {
								console.log("X1_35",indexOfNextSpace, indexOfNext,"|"+ opInp+"|", part);
								isNot=true;
								break;
							}
						}
						console.log("X1_4");
						let rawRule=opInp.substring(0,indexOfNext)
						console.log("X1_5","|"+ opInp+"|", part);
						rawRules.push(rawRule);

						opInp=opInp.substring(indexOfNext);
					}*/
					if (opInp.startsWith(part)) {//console.log("X2");
						opInp=opInp.substring(part.length);
						continue;
					} else {
						isNot=true;
						break;
					}
				} else {
					//console.log("Detected rule", part);
//					console.log("opInp"+opInp);
					let endOfRule=-1;
					if (opInp.includes(' ')) {
						endOfRule=opInp.indexOf(" ");
						//console.log("space", endOfRule);
					} else if (opInp.includes('.')) {
						endOfRule=opInp.indexOf(".");
						//console.log("dot ",endOfRule);
					}
					let rawRule=opInp.substring(0,endOfRule);
					rawRules.push(rawRule);
					opInp=opInp.substring(endOfRule);
					//console.log("rawRule",rawRule);
					//console.log("opInp",opInp);

				//	prevWasRule=true;
					continue;
				}
			}
			if (isNot) continue;


			if (dev)console.log("Found pattern for sentence: ",input, pattern);
			opInp=input;

			// ["originální slovo", "přeloženo?",  pattern.rules]
			let linkedRules=[];
			for (let rule of rawRules) {
				let p=null;
				for (let part of pattern.input) {
					if (typeof part !== 'string') {	
						p=part;
						break;
					}
				}

				linkedRules.push([rule, null, null, p]);
			}
			if (dev)console.log("Created linked rules: ", linkedRules);

			// Find translate
			for (let i=0; i<linkedRules.length; i++) {
				let link= linkedRules[i];
				let originalWord=link[0];
				let rule=link[3];
				if (rule.FallSameAsId==-1 && rule.GenderSameAsId==-1 && rule.GramaticalNumberSameAsId==-1){
					switch (rule.PartOfspeech) {
						case 1: {
							//console.log(rule.FallSameAsId==-1 , rule.GenderSameAsId==-1 , rule.GramaticalNumberSameAsId==-1);
							let z=this.searchWordNoun(originalWord.toLowerCase());
							//console.log("searchWordNoun",originalWord, z, link);
							if (z !== null) {
								linkedRules[i][2]=z; 
								console.log("searchWordNoun XXX", z);
								//console.log(z);
								//[ret, this.PatternTo.Gender, this];
								let obj=z[2];
								linkedRules[i][1]=obj.GetWordTo(rule.Number, rule.Fall);
							}else{
							//	console.log("searchWordNoun");
							}
							break;
						}
							
						case 2: {
							let z=this.searchWordAdjective(originalWord, rule.Number, rule.Fall, rule.Gender);
							//console.log("searchWordAdjective",originalWord, z, link);
							if (z !== null) {
								linkedRules[i][1]=z;
								linkedRules[i][1]=z.GetWordTo(rule.Number, rule.Fall);
							}	
							break;
						}
					}
				}
			}
			if (dev)console.log("Translated linked rules: ", linkedRules);
				
			// Solve big letters
			let r=0;
			let first=true;
			for (let out of pattern.output){
				if (typeof out === 'string') {
					first=false;
				}else{
					let final=linkedRules[r][1];
					if (final==null) {
						cancel=true;
						if (dev)console.log("Nepovedlo se detekování slova ve větě", pattern.output, linkedRules[r]);
						break;
					}
					if (first) {
					//	console.log("TTTT",this.MakeFirstLetterBig(final));
						linkedRules[r][1]=this.MakeFirstLetterBig(final); 
						first=false;
						r++;
						continue;
					}else{
						r++;
					}
				}
			}

			// Print
			r=0;
			let cancel=false;
			let ele=document.createElement("span");
			for (let out of pattern.output){
				if (typeof out === 'string') {
					this.AddText(out, ele, "sentence");
				}else{
					let final=linkedRules[r][1];
					if (final==null) {
						cancel=true;
						if (dev)console.log("Nepovedlo se detekování slova ve větě", pattern.output, linkedRules[r]);
						break;
					}
					this.AddText(final, ele, "rule");
					r++;
				}
			}
			if (!cancel)return ele;
		}
		return null;
	}

	MakeFirstLetterBig(string) {
		if (typeof string === 'string') return string.charAt(0).toUpperCase() + string.slice(1);
		if (Array.isArray(string)) {
			let arr=[];
			for (let str of string) {
				arr.push(str.charAt(0).toUpperCase() + str.slice(1));
			}
			return arr;
		}
		throw Exception("Prameter 'string' in MakeFirstLetterBig(string) has unknown typeof");
	}

	TranslateWord() {

	}

	SearchInputNounWord(stringNoun, number, fall) {
		for (let i=0; i<Nouns.length; i++){
			let noun=Nouns[i];
			if (noun.From!="") {
				if (stringNoun.startsWith(noun.input)) {
					let z=noun.GetWordFrom(number, fall);
					if (z !== null) return noun;
				}
			} else {
				let z=noun.GetWordFrom(number, fall);
				if (z !== null) return noun;
			}
		}
		return null;
	}

	SearchInputNounWord(stringNoun, number, fall, gender) {
		for (let noun of this.Nouns) {
			console.log(noun);
			if (noun.From!="") {
				if (stringNoun.startsWith(noun.From)) {
					let z=noun.GetWordFrom(number, fall, gender);
					if (z !== null) return [noun, number];
				}
			} else {
				let z=noun.GetWordFrom(number, fall, gender);
				if (z !== null) return [noun, number];
			}
		}
		return null;
	}

	AddText(x, parentElement, className){
		
		if (typeof x === "string"){
			if (!this.html) return x;
			this.AddTextOne(x, parentElement, className);
			return;
		}
		if (Array.isArray(x)) {
			if (!this.html) return x[0];
			let earr=[];
			// Remove more info
			for (let i of x) {
				if (Array.isArray(i)){
					earr.push(i[0]);
				} else earr.push(i);
			}
		//	console.log(earr);
			
			// Remove dup
			let sarr=[];
			for (let a of earr) {
				let exists=false;

				for (let s of sarr) {
					if (s == a) {
						exists = true;
						break;
					}
				}
				if (!exists) {
					sarr.push(a);
				}
			}	
			
			if (sarr.length==1) {
				this.AddTextOne(sarr[0], parentElement, className);
				return;
			}

			this.AddTextMultiple(sarr, parentElement, className);
			return;
		}
	}

	PrepareText(x){
		if (typeof x === "string") {
			return x;
		}
		if (Array.isArray(x)) {

			let earr=[];
			// Remove more info
			for (let i of x) {
				if (Array.isArray(i)) {
					if (Array.isArray(i[0])) {
						for (let a of i[0]) earr.push(a);
					} else earr.push(i[0]);
				} else earr.push(i);
			}
			
			// Remove dup
			let sarr=[];
			for (let a of earr) {
				let exists=false;

				for (let s of sarr) {
					if (s == a) {
						exists = true;
						break;
					}
				}
				if (!exists) {
					sarr.push(a);
				}
			}	
			
			if (sarr.length==1) {
				return sarr[0];
			}

			return sarr;
		}
		return "";
	}

	AddTextOne(string, parentElement, className) {
		if (this.html) {
			let span = document.createElement("span");
			span.innerText = AfterReplace(string);
			if (styleOutput) span.className = className;
			parentElement.appendChild(span);
		}else{
			parentElement+=string;
		}
	}
	
	AddSymbol(symbol, parentElement) {
		if (this.html){
			let node = document.createTextNode(symbol);
			parentElement.appendChild(node);
		}else{
			parentElement+=symbol;
		}
	}
	
	AddTextMultiple(variants, parentElement, className) {
		if (this.html){
			let pack = document.createElement("span");
			pack.className = "traMOp";

			let span = document.createElement("span");
			span.style = "text-decoration: underline dotted; cursor: pointer;";
			if (styleOutput) span.className = className;

			let box = document.createElement("ul");
			box.style = "opacity: 0";
			box.setAttribute("canHide", false);
			box.style.display = "none";
			box.className = "pop";
			let selectedIndex=0;
			for (let i = 0; i < variants.length; i++) {
				let tag = document.createElement("li");
				tag.style = "cursor: pointer;";
				tag.innerHTML = AfterReplace(variants[i]);
				tag.addEventListener('click', function () {
					selectedIndex = i;
					span.innerText = AfterReplace(variants[i]);
					box.style.opacity = "0";
					box.style.display = "none";
					box.setAttribute("canHide", false);
					setTimeout(function () { box.style.display = 'none'; }, 100);
				});
				box.appendChild(tag);
			}

			span.addEventListener('click', function () {
				if (box.style.opacity == "1") {
					box.style.opacity = "0";
					setTimeout(function () { box.style.display = 'none'; }, 100);
				} else {
					box.style.display = 'block';
					box.style.opacity = "1";
					box.setAttribute("canHide", false);
					setTimeout(function () { box.setAttribute("canHide", true); }, 100);
				}
			});

			window.addEventListener('click', function (e) {
				if (!box.contains(e.target)) {
					if (!span.contains(e.target)) {
						if (box.style.opacity == "1") {
							if (box.getAttribute("canHide")) {
								box.style.opacity = "0";
								setTimeout(function () {
									if (box.getAttribute("canHide")) {
										box.style.display = 'none';
										box.setAttribute("canHide", false);
									}
								}, 100);
							}
						}
					}
				}
			});

			span.innerText = variants[selectedIndex];

			pack.appendChild(span);
			pack.appendChild(box);
			parentElement.appendChild(pack);

			idPops++;
		}else{
				parentElement+=variants[0];
		}
	}

	MultipleSplit(string, separators) {
		let arr=[];
		let word="";
		let isSeparator;

		for (const ch of string) {
			isSeparator=false;
			//let separator;

			// Is current char separator
			for (const s of separators) {
				if (s==ch) {
					isSeparator=true;
					if (word!="") {
						arr.push([true, word]);
						word="";
					}
					arr.push([false, s]);
					break;
				}
			}

			if (!isSeparator) {
				word+=ch;
			}
		}
		if (!isSeparator) {
			if (word!="") arr.push([true, word]);
		}
		// for example [["true", "He"], [false, " "], [true, "is"], [false, " "], [true, "guy"], [false, "!"]]
		return arr;
	}
	
	SplitSentences(string, separators) {
		let arr=[];
		let sentence="";
		let isSeparator;

		for (const ch of string) {
			isSeparator=false;
		//	let separator;

			// Is current char separator
			for (const s of separators) {
				if (s==ch) {
					isSeparator=true;
					if (sentence!="") {
						sentence+=ch;
						arr.push(sentence.trim());
						sentence="";
					}
				//	arr.push([false, s]);
					break;
				}
			}

			if (!isSeparator) {
				sentence+=ch;
			}
		}
		if (!isSeparator) {
			if (sentence!="") arr.push(sentence.trim());
		}
		//console.log(arr);
		// for example [["true", "He"], [false, " "], [true, "is"], [false, " "], [true, "guy"], [false, "!"]]
		return arr;
	}

	ReplaceWord(str) {
		// Find best starting
		let bestStart=null;
		let bestStartlen=0;

		for (let s of this.ReplaceS) {
			if (str.startsWith(s.input)) {
				/*if (s.input.length<str.length) {
					if (bestStart==null) bestStart=s;
					else if (bestStart.output.length<s.input.length) {
						bestStart=s;
					}
				}*/
				if (bestStartlen<s.input.length) {
				//	if (bestStart==null) bestStart=s;
					//else if (bestStart.output.length<s.input.length) {
						bestStart=s;
						bestStartlen=s.input.length;
					//}s.input.length
				}
			}
		}
		
		// Find best ending
		let bestEnd=null;
		let maxEndLen=str.length-((bestStart==null) ? 0:bestStart.output.length);
		let bestEndLen=0;
		for (let e of this.ReplaceE) {
			if (str.endsWith(e.input)) {
				if (e.output.length<=maxEndLen) {
					if (bestEndLen<e.input.length) {
						bestEnd=e;
						bestEndLen=e.length;
					}
				}
			}
		}
		
		let endLen=((bestEnd==null) ? 0:bestEnd.input.length);

		// no inside
		if (str.length-endLen-bestStartlen==0) return bestStart?.output+bestEnd?.output;
		// Center replace (not multiple again and again)
		let inside = str.substring(bestStartlen, str.length-endLen);
		//console.log(inside);
		let PatternAlrearyReplaced=[];
		for (let i=0; i<=inside.length; i++) {
			PatternAlrearyReplaced.push("o");
		}
		let ret=inside;
		for (let g of this.ReplaceG) {
			if (inside.includes(g.input)) {
				// No overlap replaces
				let startOfReplace=inside.indexOf(g.input);
				//console.log(g.input);	
				//console.log(ret,PatternAlrearyReplaced);
				
				let doReplace=true;
				for (let i=startOfReplace; i<g.input.length; i++) {
					if (PatternAlrearyReplaced[i]!="o") {
						doReplace=false;
						break;
					}
				}

				if (doReplace) {
					ret=ret.replace(g.input, g.output);

					for (let i=startOfReplace; i<g.input.length; i++) {
						PatternAlrearyReplaced[i]="x";
					}
				}
			}			
		}

		if (bestStart==null && bestEnd==null) return ret;

		// starting only
		if (bestStart==null) {
			if (Array.isArray(bestEnd.output)) {
				let arr=[];
				for (let e of bestEnd.output) {
					arr.push(ret+e);		
				}
				console.log(arr);
				return arr;
			}else return ret+bestEnd.output;
		}

		// ending only
		if (bestEnd==null) {
			if (Array.isArray(bestStart.output)) {
				let arr=[];
				for (let s of bestEnd.output) {
					arr.push(s+ret);		
				}
				console.log(arr);
				return arr;
			} else return bestStart.output+ret;
		}

		// non null
		if (Array.isArray(bestStart.output)){
			let arr=[];
			for (let s of bestStart.output) {
				if (Array.isArray(bestEnd.output)) {
					for (let e of bestEnd.output) {
						arr.push(ret+e);		
					}
				} else return s+ret+bestEnd.output;
			}
			console.log(arr);
			return arr;
		}else{
			let arr=[];
			if (Array.isArray(bestEnd.output)) {
				for (let e of bestEnd.output) {
					arr.push(bestStart.output+ret+e);		
				}
			} else return bestStart.output+ret+bestEnd.output;
			console.log(arr);
			
			return arr;
		}	

		//return ((bestStart!=null) ?bestStart.output:"")+ret+((bestEnd!=null) ? bestEnd.output:"");
	}

	ApplyPhrases(arrOfWords, start) {
		//console.log("Phrase");

		//for (let w=start; w<start+1;/*arrOfWords.length;*/ w++) {
		//	console.log("Phrase",arrOfWords);
			for (const phrase of this.Phrases) {
				for (const variantPhrase of phrase.input) {

					//console.log("Phrase",variantPhrase);
					if (MatchArrayInArray(arrOfWords, start, variantPhrase)) {
						let ret=ApplyMatch(arrOfWords, start, start+variantPhrase.length, phrase.output);
						/*if (ret!=null)*/ return ret;
					}
				}
			}
		//}
		return null;

		function ApplyMatch(arrSource, startIndex, endIndex, arrToSet) {
			let len=endIndex-startIndex;
			console.log("ApplyMatch");

			// Verob puvodni string
			let str="";
			for (let w=startIndex; w<endIndex; w++){
				console.log(arrSource,w);
				str+=arrSource[w][1];
			}

			
			//arrSource = arrSource.filter(function (item, index){
		//		return index >= startIndex && index <= endIndex;
	//		});

			// Přidé za staré pola puvodni
			arrSource.splice(startIndex, endIndex - startIndex-1 /*+ 1*/);

			//arrSource.splice(startIndex, 0,/**/ arrToSet);
			//console.log();
			return ["Phrase", arrToSet, str];
		//	BuildingSentence.push();
			//return arrSource.insert(["Phrase", str, arrToSet]);
		}

		function MatchArrayInArray(arrSource, startIndex, arrToMatch) {
			if (arrToMatch==undefined) return false;
			//if (startIndex==0)console.log("MatchArrayInArray", arrSource, startIndex,arrToMatch);
			//if (arrSource[0]==arrToMatch[0])console.log("bene");
			if (arrSource.length-startIndex<arrToMatch.length)return false;
			for (let i=0; i+startIndex<arrSource.length && i<arrToMatch.length; i++) {
				if (arrSource[startIndex+i][1]/*.toLowerCase()*/!==arrToMatch[i]) return false;
			}
			
			return true;
		}
	}

	BuildSelect(rawStr){
	/*	if (rawStr=="") return [];

		let arr=[];
		for (const t of rawStr.split('|')) {
			let o=t.split(">");
			arr.push([o[0], o[1].split(",")]);
		}
		this.SelectReplace=arr; // = [["ł", ["ł", "u"]], ["ê", ["e", "ê"]]]*/
	}

	
}
function AfterReplace(html) {
	let ret=html;

	for (let rule of SimplyfiedReplacedRules) {
		ret.replace(rule[0], rule[1]);
	}

	return ret;
}

function PrepareReplaceRules() {
	SimplyfiedReplacedRules=[];

	let list=document.getElementById("optionsSelect");

	for (let l of list.childNodes){
		if (l.tagName=="select") {
			let replaceRule=this.SelectReplace[l.languageselectindex];
			let search=replaceRule[0];
			let replace=l.value;
			if (replace==search) continue;
			SimplyfiedReplacedRules.push([search,replace]);
		}		
	}
}