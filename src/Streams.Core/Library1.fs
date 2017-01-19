namespace Streams.Core

open FSharp.Quotations


type CardType = AtMost1 | Many
and ProducerShape<'T, 'S> =
  | For    of ProducerFor<'T, 'S>
  | Unfold of ProducerUnfold<'T, 'S>

 and ProducerFor<'T, 'S> =
    { upb :   'S -> Expr<int>;
      index : 'S -> Expr<int> -> ('T -> Expr<unit>) -> Expr<unit> }
 and ProducerUnfold<'T, 'S> =
    { term : 'S -> Expr<bool>;
      card : CardType;
      step : 'S -> ('T -> Expr<unit>) -> Expr<unit> }

and Init<'S> = 
    abstract member Invoke<'R> : ('S -> Expr<'R>) -> Expr<'R>

and StreamShape<'T> = interface end
and Stream<'T> = StreamShape<Expr<'T>>


//and Producer<'T>()