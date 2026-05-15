using System;
using HerePlatform.Blazor;
using Microsoft.JSInterop;
using NUnit.Framework;

namespace HerePlatform.Blazor.Tests;

[TestFixture]
public class ExceptionExtensionsTests
{
    [Test]
    public void AllLeavesAreOfType_SingleAllowedException_ReturnsTrue()
    {
        var ex = new JSDisconnectedException("gone");

        Assert.That(ex.AllLeavesAreOfType(typeof(JSDisconnectedException)), Is.True);
    }

    [Test]
    public void AllLeavesAreOfType_DerivedException_MatchesBaseType()
    {
        var ex = new TaskCanceledException();

        Assert.That(ex.AllLeavesAreOfType(typeof(OperationCanceledException)), Is.True);
    }

    [Test]
    public void AllLeavesAreOfType_DisallowedType_ReturnsFalse()
    {
        var ex = new InvalidOperationException("oops");

        Assert.That(ex.AllLeavesAreOfType(typeof(JSDisconnectedException)), Is.False);
    }

    [Test]
    public void AllLeavesAreOfType_AggregateWithAllAllowedLeaves_ReturnsTrue()
    {
        var ex = new AggregateException(
            new JSDisconnectedException("a"),
            new OperationCanceledException("b"));

        Assert.That(
            ex.AllLeavesAreOfType(typeof(JSDisconnectedException), typeof(OperationCanceledException)),
            Is.True);
    }

    [Test]
    public void AllLeavesAreOfType_AggregateWithMixedLeaves_ReturnsFalse()
    {
        // Reviewer finding: an AggregateException containing a real JSException
        // alongside a benign JSDisconnectedException must NOT be swallowed.
        var ex = new AggregateException(
            new JSDisconnectedException("benign"),
            new JSException("real failure"));

        Assert.That(
            ex.AllLeavesAreOfType(typeof(JSDisconnectedException), typeof(OperationCanceledException)),
            Is.False);
    }

    [Test]
    public void AllLeavesAreOfType_NestedInnerExceptions_RecursesToLeaves()
    {
        var leaf = new JSDisconnectedException("leaf");
        var wrapper = new InvalidOperationException("wrapper", leaf);

        // The wrapper itself is not allowed, but the only leaf is.
        Assert.That(wrapper.AllLeavesAreOfType(typeof(JSDisconnectedException)), Is.True);
    }

    [Test]
    public void AllLeavesAreOfType_NestedAggregate_RecursesIntoChildren()
    {
        var inner = new AggregateException(
            new JSDisconnectedException("a"),
            new TaskCanceledException());
        var outer = new AggregateException(inner);

        Assert.That(
            outer.AllLeavesAreOfType(typeof(JSDisconnectedException), typeof(OperationCanceledException)),
            Is.True);
    }

    [Test]
    public void AllLeavesAreOfType_NoAllowedTypes_ReturnsFalse()
    {
        var ex = new JSDisconnectedException("gone");

        Assert.That(ex.AllLeavesAreOfType(), Is.False);
        Assert.That(ex.AllLeavesAreOfType(Array.Empty<Type>()), Is.False);
    }

    [Test]
    public void GetLeafExceptions_SingleException_YieldsItself()
    {
        var ex = new InvalidOperationException("oops");

        var leaves = ex.GetLeafExceptions().ToList();

        Assert.That(leaves, Has.Count.EqualTo(1));
        Assert.That(leaves[0], Is.SameAs(ex));
    }

    [Test]
    public void GetLeafExceptions_AggregateOfAggregates_FlattensToLeaves()
    {
        var l1 = new JSDisconnectedException("1");
        var l2 = new TaskCanceledException();
        var l3 = new JSException("3");
        var inner = new AggregateException(l1, l2);
        var outer = new AggregateException(inner, l3);

        var leaves = outer.GetLeafExceptions().ToHashSet();

        Assert.That(leaves, Is.EquivalentTo(new Exception[] { l1, l2, l3 }));
    }

    [Test]
    public void GetLeafExceptions_CyclicGraph_TerminatesWithoutYield()
    {
        // Reviewer-Finding: pathologischer Zyklus (z.B. via Reflection) darf
        // weder eine Endlosschleife noch fälschliches Suppress-Verhalten erzeugen.
        var a = new InvalidOperationException("a");
        var b = new InvalidOperationException("b", a);
        ForceInnerException(a, b); // jetzt a → b → a (Zyklus)

        // Muss terminieren (visited-Set verhindert Endlosschleife) und keine
        // Leaves yielden (alle besuchten Knoten haben weiterführende Children).
        var leaves = a.GetLeafExceptions().ToList();
        Assert.That(leaves, Is.Empty);
    }

    [Test]
    public void AllLeavesAreOfType_CyclicGraph_ReturnsFalse()
    {
        var a = new InvalidOperationException("a");
        var b = new InvalidOperationException("b", a);
        ForceInnerException(a, b);

        // Defensive default: leerer Leaf-Set => false (nicht suppressen).
        Assert.That(a.AllLeavesAreOfType(typeof(JSDisconnectedException)), Is.False);
        Assert.That(a.AllLeavesAreOfType(typeof(InvalidOperationException)), Is.False);
    }

    private static void ForceInnerException(Exception target, Exception inner)
    {
        var field = typeof(Exception).GetField(
            "_innerException",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (field is null)
            Assert.Inconclusive("Exception._innerException field not found on this runtime");
        field!.SetValue(target, inner);
    }
}
