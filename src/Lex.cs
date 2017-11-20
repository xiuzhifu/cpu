using System;
public enum Token {
	none,
	Number,
	Char,
	String,
	db,
	dw,
	dd,
	comma,
	dot,
	leftbracket,
	rightbracket,
	minus,
	plus,
	mul,
	div,
	Ident,

	dollar,
	and,
	percent,
	colon,
    equ,

	imm,
	reg,



	end,
}

public class Lex {
	private string Source;
	private int Current;
	private int SourceLen;
	//private Token CurrentToken;
	private string TokenString;
	private int LineNo;
  private int RowNo;
	public Lex() {
	}

	public void Error(string s) {
    Console.WriteLine("(" + LineNo.ToString() + "," + RowNo.ToString() + "):" + s);
	}
	private void Skip() {
		while ((Current < SourceLen)&&(Source[Current] == ' ' || Source[Current] == '\n' || Source[Current] == '\t' || Source[Current] == '\r')) {
      if (Source[Current] == '\n') {
        LineNo++;
        RowNo = 1;
      }
      RowNo++;
			Current ++;
		}
	}

	private bool IsSkip(char c) {
		if (c == ' ' || c == '\n' || c == '\t' || c == '\r')
			return true;
		else
			return false;
	}


	public void SkipLine() {
		while (Current < SourceLen && Source[Current] != '\n') Current++;
		Current ++;
		LineNo ++;
    RowNo = 1;
  }


	private readonly char[] SingleTokenString = {
		'+',
		'-',
		'*',
		'/',
		'%',
		'&',
		'$',
		'(',
		')',
		',',
		'.',
		':',
    '=',
	};
    private readonly Token[] SingleToken = {
        Token.plus,
        Token.minus,
        Token.mul,
        Token.div,
        Token.percent,
        Token.and,
        Token.dollar,
        Token.leftbracket,
        Token.rightbracket,
        Token.comma,
        Token.dot,
        Token.colon,
        Token.equ,
	};

	public Token isSingleToken(char c){
		for (int i = 0; i < SingleTokenString.Length; i++) {
			if (SingleTokenString [i] == c)
				return SingleToken [i];
		}
		return Token.none;
	}

	private readonly string CNumber = "0123456789";
	public bool IsNumber(char c) {
		bool have = false;
    for (int i = 0; i < CNumber.Length; i++) {
      if (c == CNumber[i]) {
        have = true;
        break;
      }
    }
		if (!have)
			return false;
    else
		return true;
	}

	private readonly string CAlpha = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
	public bool IsAlpha(char c) {
		bool have = false;
		for (int i = 0; i < CAlpha.Length; i++) {
			if (c == CAlpha [i]) {
				have = true;
				break;
			}
		}
    if (have)
      return true;
    else
      return false;
	}

	public void Load(string s) {
		Source = s;
		Current = 0;
		LineNo = 1;
    RowNo = 1;
    SourceLen = s.Length;
	}

	public Token NextToken(bool match = false) {	
		Skip();
    int current = Current;
    int rowno = RowNo;
    if (current >= SourceLen) return Token.end;
		Token token = isSingleToken (Source[current]);
		if (token != Token.none) {
			TokenString = Source [current].ToString();
      current++;
      rowno++;
      if (match) {
        Current = current;
        RowNo = rowno;
      }
			return token;
		}
		int s = Current;
		int i = 1;
		if (Source[current] == '"') {
      current++;
			while ((current < SourceLen) && Source[current] != '"' && Source[current] != '\n') current++;
			if (current >= SourceLen || Source[current] == '\n') {
				Error("couple of \" expect but only found one");
				return NextToken();
			}
      current++;
			TokenString = Source.Substring(s, current - s);
      rowno = rowno + (current - s);
      if (match) {
        Current = current;
        RowNo = rowno;
      }
      return Token.String;		
		} else if (Source[current] == '\'') {
			if (current + 2 < SourceLen && Source[current + 2] == '\'') {
				TokenString = Source[current + 1].ToString();
        current = current + 3;
        rowno = rowno + 3;
        if (match) {
          Current = current;
          RowNo = rowno;
        }
        return Token.Char;
			} else {
				Error("like 'A', only one char can in a couple of '");
				SkipLine();
				return NextToken();
			}
		}
		else if (IsNumber(Source[current]) || ((i == 2) && (Source[current] == 'x' || Source[current] == 'X'))) {
			i ++;
      current++;
			while (current < SourceLen && !IsSkip(Source[current]) && isSingleToken(Source[current]) == Token.none) {
				if (IsNumber(Source[current])) current++;
				else {
					Error(Source[current] + "can't in number");
					SkipLine();
					return NextToken();
				}
			}
			TokenString = Source.Substring (s, current - s);
      rowno = rowno + (current - s);
      if (match) {
        Current = current;
        RowNo = rowno;
      }
      return Token.Number;
		} else if (IsAlpha(Source[current]) || Source[current] == '_') {
      current++;
			while (current < SourceLen && !IsSkip(Source[current]) && isSingleToken(Source[current]) == Token.none) {
				if (IsNumber(Source[current]) || IsAlpha(Source[current]) || Source[current] == '_') current++;
				else {
					Error(Source[current] + " can't in identifier");
					SkipLine();
					return NextToken();
				}
			}
			TokenString = Source.Substring (s, current - s);
      rowno = rowno + (current - s);
      if (match) {
        Current = current;
        RowNo = rowno;
      }
      return Token.Ident;
		}
		return Token.none;
		//if (Current >= SourceLen && s == Current) return Token.end;
	}

	public string GetTokenString() {
		return TokenString;
	}

	public int GetLineNo() {
		return LineNo;
	}

  public int GetRowNo() {
    return RowNo;
  }
    public bool Match(Token token) {
        if (NextToken() == token) { 
            NextToken(true);
            return true;
        }
        else {
            Error("unmatch " + token.ToString());
            return false;
        }
			
	}
}


