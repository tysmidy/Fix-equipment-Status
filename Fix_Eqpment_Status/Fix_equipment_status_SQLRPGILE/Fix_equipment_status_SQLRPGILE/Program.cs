/ Free
//Declare files that will be used
// Declare files names, pull the actual names from the database, they are read only and can be searc

Dcl - f EQPMASFL Disk(*ext) Usage(*input) Keyed;
Dcl - f SYSLOCFL Disk(*ext) Usage(*input) Keyed

//declare variables
Dcl-s eqpStatus like (emstat);
Dcl - s userName Char(10) inz(*User);
Dcl - s cmpCode like(SCCMP);
Dcl - c userLoc Like(ZLLOC);
Dcl - c fileName 'EQPMASFL  ';
Dcl - c fieldName 'EMSTAT';

//may want to add some error handling or log variables here
Dcl - s err Char(1)  inz('N');

//declare data structure
Dcl - ds eqpmasFields;
SCCMP Char(2);
SCEQP# Char(10);

//Clear error flags if they are on
//These are not declared except err
//May need to initialize another program within this or name these error flags

If(*IN30 = *on);
*IN30 = *off;
Clear err;
Clear ScErrD;
*In25 = *off;
*In26 = *off;
*In27 = *off;
*In28 = *off;
*In29 = *off;
Endif;

//cmp code = data structure declared var
cmpCode = SCCMP;

//Check for valid company code
If SCCMP = *Blanks or



//begin subroutine and execute the code

Begsr Process;


//Execute SQL to grab all the records. Add a cursor to execute the SQL and go through the program an
//the equipment status until there are 0 records.

/ Exec Sql
Declare BrokenStatusRecord Cursor for
Select EMSTAT
Into eqpStatus
Where Emcmp = :cmpCode
And emeqp# = :sceeqp#
And EMSTAT = '1'
Or EMSTAT = '2';

//open the cursor
/ Exec Sql
        Open BrokenStatusRecord;
/ Exec Sql
        Fetch Next From BrokenStatusRecord into eqpStatus;

While SqlCode = 0;

//change to status A

Exec SQL
Declare StatusA Cursor for
Select EMEQP#
from RAODETFL
Join RACHDRFL
ON RDCMP = RHCMP
and RDCON# = RHCON#
and RDLOC = RHLOC
and RDTYPE = RHTYPE
Where AND A.RHTYPE NOT IN ('R','X','Q','Y','T');

//open the cursor
Exec Sql
        Open StatusA;

//run SQL statements , if count is = 1, fix the status, else if move on to the next method

if SqlCode = 0 and Count >= 1 and SCCMP<> *Blanks or 'ZZ' or 'ZT' ;
start itr;
EMSTAT = 'A';

        //iterate to make sure that there are no more records//

        / Exec Sql
        Fetch Next From StatusA into iterateOne;
End itr;
Endif;


//change to status S
Else;
Exec Sql
          Declare StatusS Cursor for
          Select RDITEM
          From RAOCDETFL
          Join RACHDRFL
          on RDCMP = RHCMP
          and RDCON = RHCON
          and RDLOC = RHLOC
          and RDTYPE = RHTYPE
          Where RDTYPE = 'ES'
          AND RHTYPE = 'ES';

        //open the cursor
        / Exec Sql
        Open StatusA;

if SqlCode = 0 and Count >= 1 and SCCMP<> *Blanks or 'ZZ' or 'ZT' ;
start itr;
EMSTAT = 'S';

              //iterate to make sure that there are no more records//
                / Exec Sql
                  Fetch Next From StatusA into iterateOne;
End itr;
Endif;

//change status to U (on pickup)
Else;
Exec Sql
          Declare StatusU Cursor for
          Select EMEQP#
          from RAOCDETFL
          join RAPKUPFL
          on RDCMP = RUCMP
          and RDCON = RUCON#
          and EMEQP# = RUEQP#
          Where RUSTTS = 'OP';

//Open the cursor
Open StatusU;

if SqlCode = 0 and Count >= 1 and SCCMP<> *Blanks or 'ZZ' or 'ZT';
start itr;
EMSTAT = 'U';

              //iterate to make sure that there are no more records//
                / Exec Sql
                  Fetch Next From StatusU into iterateOne;
End itr;
Endif;

//change status to D (Down)
Else;
Exec SQL
            Declare StatusD Cursor for
            Select EMEQP#
            from



/Exec Sql
        Fetch Next From BrokenStatusRecord into eqpStatus;






//while SqlCode = 0 and Count <= 1;
//Exec Sql

