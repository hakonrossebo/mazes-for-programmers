
type NodeId = Int

type Edge<'e> = Edge of NodeId * NodeId * 'e

type Node<'a, 'e> =
    { id : NodeId
    ; value : 'a
    ; north :  (Edge<'e>) option
    ; east :  (Edge<'e>) option
    ; south :  (Edge<'e>) option
    ; west :  (Edge<'e>) option
    }

type QuadGraph<'a, 'e> = Map<NodeId, Node<'a, 'e>>

type MazeGraph = QuadGraph<unit, bool>

