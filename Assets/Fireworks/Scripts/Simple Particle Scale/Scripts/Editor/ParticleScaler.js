/****************************************
	ParticleScaler.js v1.3
	Copyright 2013-2015 Unluck Software	
 	WWW.chemicalbliss.com																			
*****************************************/
class ParticleScaler extends EditorWindow {
    var _scaleMultiplier: float = 1.0f;
    var _maxParticleScale: float = .5f;
    var _maxParticles: int = 50;
    var _origScale: float = 1;
	var _autoRename:boolean;
    @MenuItem("Window/Simple Particle Scaler")

    static function ShowWindow() {
        var win = EditorWindow.GetWindow(ParticleScaler);
        win.title = "PS Scaler";
        win.minSize = new Vector2(300, 165);
        win.maxSize = new Vector2(300, 165);
    }

    function OnGUI() {
        var txt: String = "Select Particle Systems";
        if (Selection.gameObjects.length == 1)
            txt = Selection.gameObjects[0].name;
        else if (Selection.gameObjects.length > 1)
            txt = "" + Selection.gameObjects.length + " Selected Particle Systems";
        EditorGUILayout.LabelField("Shuriken Particle System Scaler", EditorStyles.boldLabel);
        EditorGUILayout.LabelField(txt, EditorStyles.miniLabel);
        EditorGUILayout.Space();
        _scaleMultiplier = EditorGUILayout.Slider(_scaleMultiplier, 0.1f, 4.0f);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Scale", EditorStyles.miniButtonLeft)) {
            for (var gameObj: GameObject in Selection.gameObjects) {             
                gameObj.transform.localScale *= _scaleMultiplier;
				if(this._autoRename){
	            	var s = gameObj.name.Split("¤"[0]);
	            	if(s.Length==1){
	            		gameObj.name += " ¤" + _scaleMultiplier;
	            	}else{               
	            		var i:float = float.Parse(s[s.Length-1]);                        	
	            		gameObj.name = s[0] +"¤" + _scaleMultiplier*i;
	            	}
                }
                var pss: ParticleSystem[];
                pss = gameObj.GetComponentsInChildren. < ParticleSystem > ();
                for (var ps: ParticleSystem in pss) {
                    ps.Stop();
                    scalePs(gameObj, ps);
                    ps.Play();
                }
            }
        }
        EditorStyles.miniButtonMid.fixedHeight = 15;
        if (GUILayout.Button("Play", EditorStyles.miniButtonMid)) {
            playParticles();
        }
        if (GUILayout.Button("Stop", EditorStyles.miniButtonRight)) {
            stopParticles();
        }
        EditorGUILayout.EndHorizontal();
        //        	if (GUILayout.Button("Refresh", EditorStyles.miniButtonMid)) {
        //        	updateParticles();
        //        }
        //        	 EditorGUILayout.EndHorizontal();
      
      	//         _maxParticleScale = EditorGUILayout.Slider(_maxParticleScale, 0.01, 2);
        //        //EditorGUILayout.LabelField("Max Particle Scale", EditorStyles.boldLabel);
        //        if (GUILayout.Button("Set" , EditorStyles.miniButtonMid)) {
        //        	maxScale();
        //        }
        //        EditorGUILayout.LabelField("Change Max Particles in Selection");
        //        if (GUILayout.Button("Max Particles")) {
        //        	maxParticles();
        //        }
        //        _maxParticles = EditorGUILayout.Slider(_maxParticles, 10, 10000);
        EditorGUILayout.BeginHorizontal();
        _autoRename = GUILayout.Toggle(_autoRename, "Automatic rename");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Prefabs")) {
            DoCreateSimplePrefab();
        }
        EditorGUILayout.EndHorizontal();
   		EditorGUILayout.LabelField("( Avoid Using Symbols In Name)" , EditorStyles.miniLabel);
    }

    function DoCreateSimplePrefab() {
        if (Selection.gameObjects.Length > 0) {
            var path = EditorUtility.SaveFolderPanel("Select Folder ", "Assets/", "");
            if (path.Length > 0) {
                if (path.Contains("" + Application.dataPath)) {
                    var s: String = "" + path + "/";
                    var d: String = "" + Application.dataPath + "/";
                    var p: String = "Assets/" + s.Remove(0, d.Length);
                    var objs = Selection.gameObjects;
                    var cancel: boolean;
                    for (var go: GameObject in objs) {
                        if (!cancel) {
                            if (AssetDatabase.LoadAssetAtPath(p + go.gameObject.name + ".prefab", GameObject)) {
                                var option = EditorUtility.DisplayDialogComplex("Are you sure?", "" + go.gameObject.name + ".prefab" + " already exists. Do you want to overwrite it?", "Yes", "No", "Cancel");

                                switch (option) {
                                case 0:
                                    CreateNew(go.gameObject, p + go.gameObject.name + ".prefab");
                                case 1:
                                    break;
                                case 2:
                                    cancel = true;
                                    break;
                                default:
                                    Debug.LogError("Unrecognized option.");
                                }

                            } else
                                CreateNew(go.gameObject, p + go.gameObject.name + ".prefab");
                        }
                    }
                } else {
                    Debug.LogError("Prefab Save Failed: Can't save outside project: " + path);
                }
            }
        } else {
            Debug.LogWarning("No GameObjects Selected");
        }
    }

    static function CreateNew(obj: GameObject, localPath: String) {
        var prefab: Object = PrefabUtility.CreateEmptyPrefab(localPath);
        PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
    }

    function updateParticles() {
        for (var gameObj: GameObject in Selection.gameObjects) {
            var pss: ParticleSystem[];
            pss = gameObj.GetComponentsInChildren. < ParticleSystem > ();
            for (var ps: ParticleSystem in pss) {
                ps.Stop();
                ps.Play();
            }
        }
    }

    function stopParticles() {
        for (var gameObj: GameObject in Selection.gameObjects) {
            var pss: ParticleSystem[];
            pss = gameObj.GetComponentsInChildren. < ParticleSystem > ();
            for (var ps: ParticleSystem in pss) {
                ps.Stop();
            }
        }
    }

    function playParticles() {
        for (var gameObj: GameObject in Selection.gameObjects) {
            var pss: ParticleSystem[];
            pss = gameObj.GetComponentsInChildren. < ParticleSystem > ();
            for (var ps: ParticleSystem in pss) {
                ps.Play();
            }
        }
    }

    function maxParticles() {
        for (var gameObj: GameObject in Selection.gameObjects) {
            var pss: ParticleSystem[];
            pss = gameObj.GetComponentsInChildren. < ParticleSystem > ();
            for (var ps: ParticleSystem in pss) {
                ps.Stop();
                var sObj: SerializedObject = new SerializedObject(ps);
                sObj.FindProperty("InitialModule.maxNumParticles").intValue = _maxParticles;
                sObj.ApplyModifiedProperties();
                ps.Play();
            }
        }
    }

    function maxScale() {
        for (var gameObj: GameObject in Selection.gameObjects) {

            var pss: ParticleSystemRenderer[];
            pss = gameObj.GetComponentsInChildren. < ParticleSystemRenderer > ();
            for (var ps: ParticleSystemRenderer in pss) {
                ps.particleSystem.Stop();
                ps.maxParticleSize = _maxParticleScale;
                ps.particleSystem.Play();
            }
        }
    }

    function scalePs(__parent: GameObject, __particles: ParticleSystem) {
        if (__parent != __particles.gameObject) {
            __particles.transform.localPosition *= _scaleMultiplier;
        }
        __particles.startSize *= _scaleMultiplier;
        __particles.gravityModifier *= _scaleMultiplier;
        __particles.startSpeed *= _scaleMultiplier;
        var sObj: SerializedObject = new SerializedObject(__particles);
        sObj.FindProperty("ShapeModule.boxX").floatValue *= _scaleMultiplier;
        sObj.FindProperty("ShapeModule.boxY").floatValue *= _scaleMultiplier;
        sObj.FindProperty("ShapeModule.boxZ").floatValue *= _scaleMultiplier;
        sObj.FindProperty("ShapeModule.radius").floatValue *= _scaleMultiplier;
        sObj.FindProperty("VelocityModule.x.scalar").floatValue *= _scaleMultiplier;
        sObj.FindProperty("VelocityModule.y.scalar").floatValue *= _scaleMultiplier;
        sObj.FindProperty("VelocityModule.z.scalar").floatValue *= _scaleMultiplier;
        scaleCurve(sObj.FindProperty("VelocityModule.x.minCurve").animationCurveValue);
        scaleCurve(sObj.FindProperty("VelocityModule.x.maxCurve").animationCurveValue);
        scaleCurve(sObj.FindProperty("VelocityModule.y.minCurve").animationCurveValue);
        scaleCurve(sObj.FindProperty("VelocityModule.y.maxCurve").animationCurveValue);
        scaleCurve(sObj.FindProperty("VelocityModule.z.minCurve").animationCurveValue);
        scaleCurve(sObj.FindProperty("VelocityModule.z.maxCurve").animationCurveValue);
        sObj.FindProperty("ClampVelocityModule.x.scalar").floatValue *= _scaleMultiplier;
        sObj.FindProperty("ClampVelocityModule.y.scalar").floatValue *= _scaleMultiplier;
        sObj.FindProperty("ClampVelocityModule.z.scalar").floatValue *= _scaleMultiplier;
        sObj.FindProperty("ClampVelocityModule.magnitude.scalar").floatValue *= _scaleMultiplier;
        scaleCurve(sObj.FindProperty("ClampVelocityModule.x.minCurve").animationCurveValue);
        scaleCurve(sObj.FindProperty("ClampVelocityModule.x.maxCurve").animationCurveValue);
        scaleCurve(sObj.FindProperty("ClampVelocityModule.y.minCurve").animationCurveValue);
        scaleCurve(sObj.FindProperty("ClampVelocityModule.y.maxCurve").animationCurveValue);
        scaleCurve(sObj.FindProperty("ClampVelocityModule.z.minCurve").animationCurveValue);
        scaleCurve(sObj.FindProperty("ClampVelocityModule.z.maxCurve").animationCurveValue);
        scaleCurve(sObj.FindProperty("ClampVelocityModule.magnitude.minCurve").animationCurveValue);
        scaleCurve(sObj.FindProperty("ClampVelocityModule.magnitude.maxCurve").animationCurveValue);
        sObj.FindProperty("ForceModule.x.scalar").floatValue *= _scaleMultiplier;
        sObj.FindProperty("ForceModule.y.scalar").floatValue *= _scaleMultiplier;
        sObj.FindProperty("ForceModule.z.scalar").floatValue *= _scaleMultiplier;
        scaleCurve(sObj.FindProperty("ForceModule.x.minCurve").animationCurveValue);
        scaleCurve(sObj.FindProperty("ForceModule.x.maxCurve").animationCurveValue);
        scaleCurve(sObj.FindProperty("ForceModule.y.minCurve").animationCurveValue);
        scaleCurve(sObj.FindProperty("ForceModule.y.maxCurve").animationCurveValue);
        scaleCurve(sObj.FindProperty("ForceModule.z.minCurve").animationCurveValue);
        scaleCurve(sObj.FindProperty("ForceModule.z.maxCurve").animationCurveValue);
        sObj.ApplyModifiedProperties();
    }

    function scaleCurve(curve: AnimationCurve) {
        for (var i: int = 0; i < curve.keys.Length; i++) {
            curve.keys[i].value *= _scaleMultiplier;
        }
    }
}