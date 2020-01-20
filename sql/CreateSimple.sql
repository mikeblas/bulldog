

CREATE TABLE SimpleSource
(
IntKey INTEGER NOT NULL,
SomeName VARCHAR(48),
WhenDateTime DATETIME
);

CREATE TABLE SimpleDest
(
IntKey INTEGER NOT NULL,
SomeName VARCHAR(48),
WhenDateTime DATETIME
);

INSERT INTO SimpleSource (IntKey, SomeName, WhenDateTime) VALUES
(2, 'Mike', '2011-10-13'),
(3, 'Barney', '2019-05-02'),
(4, 'Betty', '2005-02-22'),
(5, 'Wilma', '1999-06-05'),
(6, 'Fred', '2020-01-14');


