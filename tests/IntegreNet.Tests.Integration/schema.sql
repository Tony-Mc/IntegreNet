CREATE TABLE customers (
    id      serial,
    name    varchar(40),

    PRIMARY KEY (id)
);

INSERT INTO customers(name) VALUES
    ('Bob'),
    ('Stuart'),
    ('Steve')