set -e

psql -v ON_ERROR_STOP=1 --username "$postgres" --dbname "$LibDB" <<-EOSQL
    CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
EOSQL
