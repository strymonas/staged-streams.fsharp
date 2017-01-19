namespace Streams.Core

open FSharp.Quotations


type CardType = AtMost1 | Many

and ProducerShape<'T, 'S> =
  | For    of ProducerFor<'T, 'S>
  | Unfold of ProducerUnfold<'T, 'S>
and ProducerFor<'T, 'S> =
    { Upb :   'S -> Expr<int>;
      Index : 'S -> Expr<int> -> ('T -> Expr<unit>) -> Expr<unit> }
 and ProducerUnfold<'T, 'S> =
    { Term : 'S -> Expr<bool>;
      Card : CardType;
      Step : 'S -> ('T -> Expr<unit>) -> Expr<unit> }

and Init<'S> = 
    abstract member Invoke<'R> : ('S -> Expr<'R>) -> Expr<'R>

and ProducerUnPack<'T, 'R> = 
    abstract Invoke<'S> : Init<'S> * ProducerShape<'T, 'S> -> 'R
and Producer<'T> = 
    abstract Invoke<'R> : ProducerUnPack<'T, 'R> -> 'R
and ProducerConstr<'S, 'T>(init : Init<'S>, producerShape : ProducerShape<'T, 'S>) = 
    interface Producer<'T> with
        member self.Invoke<'R> (producerUnPack : ProducerUnPack<'T, 'R>) = 
            producerUnPack.Invoke<'S>(init, producerShape)

and StreamShapeUnPack<'T, 'R> = 
    abstract Invoke : Producer<'T> -> 'R
    abstract Invoke<'U> : Producer<'U> * ('U -> Producer<'T>) -> 'R
and StreamShape<'T> = 
    abstract Invoke<'R> : StreamShapeUnPack<'T, 'R> -> 'R
and Linear<'T>(producer : Producer<'T>) = 
    interface StreamShape<'T> with
        member self.Invoke<'R>(streamShapeUnPack : StreamShapeUnPack<'T, 'R>) =
            streamShapeUnPack.Invoke(producer)
and Nestead<'T, 'U>(producer : Producer<'U>, f : 'U -> Producer<'T>) = 
    interface StreamShape<'T> with
        member self.Invoke<'R>(streamShapeUnPack : StreamShapeUnPack<'T, 'R>) = 
            streamShapeUnPack.Invoke<'U>(producer, f)

and Stream<'T> = StreamShape<Expr<'T>>

