#include "graph.h"

struct edge {
       int v;
       elem peso;
       Edge* prox;
};

struct vertex {
       Edge *start, *end;
};

struct graph {
       Vertex adj[n];
       int nVertex;
};

Graph* New_Graph(Graph* g, int n) {

       Graph* g = malloc(sizeof(Graph));
       g->nVertex = n;

       for (int i = 0; i < g->nVertex; i++)
              g->adj[i] = malloc(sizeof(Vertex));
}

Graph* Graph_Insert() {

}
