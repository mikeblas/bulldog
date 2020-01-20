grammar bulldog;

file : declaration* EOF;

fragment A          : ('A'|'a') ;
fragment B          : ('B'|'b') ;
fragment C          : ('C'|'c') ;
fragment D          : ('D'|'d') ;
fragment E          : ('E'|'e') ;
fragment H          : ('H'|'h') ;
fragment I          : ('I'|'i') ;
fragment J          : ('J'|'j') ;
fragment K          : ('K'|'k') ;
fragment L          : ('L'|'l') ;
fragment M          : ('M'|'m') ;
fragment N          : ('N'|'n') ;
fragment O          : ('O'|'o') ;
fragment P          : ('P'|'p') ;
fragment Q          : ('Q'|'q') ;
fragment R          : ('R'|'r') ;
fragment S          : ('S'|'s') ;
fragment T          : ('T'|'t') ;
fragment U          : ('U'|'u') ;
fragment V          : ('V'|'v') ;
fragment W          : ('W'|'w') ;
fragment X          : ('W'|'x') ;
fragment Y          : ('Y'|'y') ;
fragment Z          : ('Z'|'z') ;

fragment LOWERCASE  : [a-z] ;
fragment UPPERCASE  : [A-Z] ;
fragment DIGITS     : [0-9] ;

// NEWLINE             : ('\r'? '\n' | '\r')+ ;

TYPE_DATA_SOURCE : 'SOURCE';
TYPE_DATA_DESTINATION : 'DESTINATION';
TYPE_AGGREGATE : 'AGGREGATE';

quotedString : QSTRING ;

// a string surrounded with quotes, which can have \" escapes for inside quote marks
QSTRING : '"' (~[\\"] | '\\' [\\"()])* '"' ;

READ_FROM: 'READ FROM';

WRITE_TO: 'WRITE TO';

INTO_TABLE: 'INTO TABLE';

USING_CONNECT_STRING: 'USING CONNECT STRING';

FROM_FILE: 'FROM FILE';

FROM_TABLE: 'FROM TABLE';

EXECUTE: 'EXECUTE';

WITH_INPUT_FROM: 'WITH INPUT FROM';

DECLARE: 'DECLARE';

AS: 'AS';

BEGIN: 'BEGIN';

END: 'END';

COMMA: ',';

SQUARE_OPEN: '[';

SQUARE_CLOSED: ']';

STAR: '*';

COLUMNS: 'COLUMNS';

column: WORD;

column_list: SQUARE_OPEN column (COMMA column)* SQUARE_CLOSED;

star_column_list: SQUARE_OPEN STAR SQUARE_CLOSED;

columns: COLUMNS (column_list | star_column_list);

read_from: READ_FROM s=WORD;

write_to: WRITE_TO s=WORD;

using_connect: USING_CONNECT_STRING s=quotedString;

from_file: FROM_FILE s=quotedString;

execute: EXECUTE s=quotedString;

into_table: INTO_TABLE s=WORD;

with_input_from: WITH_INPUT_FROM s=WORD;

from_table: FROM_TABLE s=WORD;

source_declaration: DECLARE w=WORD AS TYPE_DATA_SOURCE BEGIN (read_from | using_connect | from_file | execute | from_table | columns)* END;

dest_declaration: DECLARE w=WORD AS TYPE_DATA_DESTINATION BEGIN (with_input_from | using_connect | write_to | execute | into_table | columns)* END;

aggregate_declaration: DECLARE w=WORD AS TYPE_AGGREGATE BEGIN (with_input_from)* END;

declaration: (source_declaration | dest_declaration | aggregate_declaration);

// declaration: DECLARE w=WORD AS mytype=(TYPE_DATA_SOURCE | TYPE_DATA_DESTINATION) BEGIN ;

WORD: (LOWERCASE | UPPERCASE | '_' | DIGITS)+ ;

WS: [ \r\n\t] + -> skip ;

