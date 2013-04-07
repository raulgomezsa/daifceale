#!/bin/bash

    # Copyright Manuel Palomo-Duarte, 2013

    # This program is free software: you can redistribute it and/or modify
    # it under the terms of the GNU General Public License as published by
    # the Free Software Foundation, either version 3 of the License, or
    # (at your option) any later version.

    # This program is distributed in the hope that it will be useful,
    # but WITHOUT ANY WARRANTY; without even the implied warranty of
    # MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    # GNU General Public License for more details.

    # You should have received a copy of the GNU General Public License
    # along with this program.  If not, see <http://www.gnu.org/licenses/>.

SINTAXIS="$0 <fichero_log_chat.csv>"

if [ $# -ne 1 ]
then
 echo $SINTAXIS
 exit
fi

if [ ! -f $1 ]
then
 echo "Error, el fichero $1 no existe o no se puede leer"
 exit
fi

# Un turno es el paso de hablar uno a hablar otro
NT=$(cat $1 | cut -d";" -f2 | uniq | wc -l)
echo "Number of turns: $NT"

NP=$(cat $1 | cut -d";" -f2 | wc -l)
echo "Phrases: $NP"

#LT=$(cat $1 | cut -d";" -f2,3)
#echo "Length of turn: $LT"

# Frases de una sola palabra
echo -n "Single words: $SW"
cat $1 | cut -d";" -f3 | awk '{print $2}'|grep -v '^$'|wc -l

TW=$(cat $1 | cut -d";" -f3 | wc -w)
echo "Total words: $TW"

#echo "German-only total words: "
