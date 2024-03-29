using Unity.Entities;

using UnityEngine;

public class AnimationSystem : SystemBase
{
    BattleSystem battleSystem;
    TransitionSystem transitionSystem;
    protected override void OnCreate()
    {
        battleSystem = World.GetOrCreateSystem<BattleSystem>();
        battleSystem.OnBattleEnd += SheathWeapon_OnBattleEnd;
        battleSystem.OnBattleStart += StartTransitionAnim_OnBattleStart;
        transitionSystem = World.GetOrCreateSystem<TransitionSystem>();
    }
    protected override void OnUpdate()
    {
        Entities
        .WithoutBurst()
        .WithNone<BattleData>()
        .ForEach((Animator animator, AnimationData animationData, in MovementData movement) =>{
            if(animator.isActiveAndEnabled){
                if(movement.direction.x == 0 && movement.direction.y == 0){
                    if(!animationData.inIdleAnimation){
                        switch(animationData.previouslyFacing){
                            case Direction.up:
                                animationData.inIdleAnimation = true;
                                animator.Play(animationData.idleUpAnimationName);
                                break;
                            case Direction.down:
                                animationData.inIdleAnimation = true;
                                animator.Play(animationData.idleDownAnimationName);
                                break;
                            case Direction.left:
                                animationData.inIdleAnimation = true;
                                animator.Play(animationData.idleLeftAnimationName);
                                break;
                            case Direction.right:
                                animationData.inIdleAnimation = true;
                                animator.Play(animationData.idleRightAnimationName);
                                break;
                        }
                    }
                }
                else if(Mathf.Abs(movement.direction.x) > Mathf.Abs(movement.direction.y)){
                    animationData.inIdleAnimation = false;
                    if(movement.direction.x > 0){
                        
                        animationData.previouslyFacing = Direction.right;
                        animator.Play(animationData.walkRightAnimationName);
                    }
                    else{
                        animationData.previouslyFacing = Direction.left;
                        animator.Play(animationData.walkLeftAnimationName);
                    }
                }
                else{
                    animationData.inIdleAnimation = false;
                    if(movement.direction.y > 0){
                        animationData.previouslyFacing = Direction.up;
                        animator.Play(animationData.walkUpAnimationName);
                    }
                    else{
                        animationData.previouslyFacing = Direction.down;
                        animator.Play(animationData.walkDownAnimationName);
                    }
                }
            }
            
        }).Run();

        Entities
        .WithoutBurst()
        .ForEach((Animator animator, AnimationData animationData, in MovementData movement, in PlayerSelectorUI playerSelectorUI, in BattleData battleData) =>{
            /*
            if (animationData.hasTakenDamage){
                if(animationData.takenDamageAnimationName != ""){
                    animator.Play(animationData.takenDamageAnimationName);
                }
            }
            else{
                switch(battleData.itemType){
                    case ItemType.none:
                        if(animationData.fistIdleAnimationName != "" && !(animationData.currentAnimation == animationData.fistIdleAnimationName)){
                                animator.Play(animationData.fistIdleAnimationName);
                                animationData.currentAnimation = animationData.fistIdleAnimationName;
                        }
                        break;
                    case ItemType.sword:
                        if(animationData.swordIdleAnimationName != "" && !(animationData.currentAnimation == animationData.swordIdleAnimationName)){
                                animator.Play(animationData.swordIdleAnimationName);
                                animationData.currentAnimation = animationData.swordIdleAnimationName;
                        }
                        break;
                    
                }
            }
            */
        }).Run();
        
    }
    private void DrawWeapon_OnTransitionEnd(System.Object sender, System.EventArgs e){
        foreach(Entity entity in battleSystem.playerEntities){
            Animator animator = EntityManager.GetComponentObject<Animator>(entity);
            AnimationData animation = EntityManager.GetComponentObject<AnimationData>(entity);
            animator.Play(animation.basicSwordAnimationName);
        }
        transitionSystem.OnTransitionEnd -= DrawWeapon_OnTransitionEnd;
    }
    private void StartTransitionAnim_OnBattleStart(System.Object sender, System.EventArgs e){
        foreach(Entity entity in battleSystem.playerEntities){
            Animator animator = EntityManager.GetComponentObject<Animator>(entity);
            AnimationData animation = EntityManager.GetComponentObject<AnimationData>(entity);
            animator.Play(animation.idleRightAnimationName);
        }
        transitionSystem.OnTransitionEnd += DrawWeapon_OnTransitionEnd;
    }
    private void SheathWeapon_OnBattleEnd(System.Object sender, OnBattleEndEventArgs e){
        if(e.isPlayerVictor){
            foreach(Entity entity in battleSystem.playerEntities){
                Animator animator = EntityManager.GetComponentObject<Animator>(entity);
                AnimationData animation = EntityManager.GetComponentObject<AnimationData>(entity);
                animator.Play(animation.idleRightAnimationName);
            }
        }
        
    }
}

public enum Direction{
    up,
    down,
    right,
    left
}
public enum AnimationType{
    idle,
    attack,
    walk,
    running
}
